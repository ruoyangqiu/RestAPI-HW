using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using restapi.Helpers;

namespace restapi.Models
{
    public class Timecard
    {
        public Timecard() { }

        public Timecard(int person)
        {
            Opened = DateTime.UtcNow;
            Employee = person;
            UniqueIdentifier = Guid.NewGuid();
            Lines = new List<TimecardLine>();
            Transitions = new List<Transition>();
        }

        // change set method of the employee to private to 
        public int Employee { get; private set; }

        public TimecardStatus Status
        {
            get
            {
                return Transitions
                    .OrderByDescending(t => t.OccurredAt)
                    .First()
                    .TransitionedTo;
            }
        }

        [BsonIgnore]
        [JsonProperty("_self")]
        public string Self { get => $"/timesheets/{UniqueIdentifier}"; }

        public DateTime Opened { get; set; }

        [JsonIgnore]
        [BsonId]
        public ObjectId Id { get; set; }

        [JsonProperty("id")]
        public Guid UniqueIdentifier { get; set; }

        [JsonIgnore]
        public IList<TimecardLine> Lines { get; set; }

        [JsonIgnore]
        public IList<Transition> Transitions { get; set; }

        public IList<ActionLink> Actions { get => GetActionLinks(); }

        [JsonProperty("documentation")]
        public IList<DocumentLink> Documents { get => GetDocumentLinks(); }

        public string Version { get; set; } = "timecard-0.1";

        // Add a link under Draft timecardstatus to delete the timecard
        // Add a link under Canceled timecardstatus to delete the timecard
        private IList<ActionLink> GetActionLinks()
        {
            var links = new List<ActionLink>();

            switch (Status)
            {
                case TimecardStatus.Draft:
                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.Cancellation,
                        Relationship = ActionRelationship.Cancel,
                        Reference = $"/timesheets/{UniqueIdentifier}/cancellation"
                    });

                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.Submittal,
                        Relationship = ActionRelationship.Submit,
                        Reference = $"/timesheets/{UniqueIdentifier}/submittal"
                    });

                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.TimesheetLine,
                        Relationship = ActionRelationship.RecordLine,
                        Reference = $"/timesheets/{UniqueIdentifier}/lines"
                    });

                    links.Add(new ActionLink()
                    {
                        Method = Method.Delete,
                        Type = ContentTypes.Deletion,
                        Relationship = ActionRelationship.Delete,
                        Reference = $"/timesheets/{UniqueIdentifier}/deletion"
                    });

                    break;

                case TimecardStatus.Submitted:
                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.Cancellation,
                        Relationship = ActionRelationship.Cancel,
                        Reference = $"/timesheets/{UniqueIdentifier}/cancellation"
                    });

                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.Rejection,
                        Relationship = ActionRelationship.Reject,
                        Reference = $"/timesheets/{UniqueIdentifier}/rejection"
                    });

                    links.Add(new ActionLink()
                    {
                        Method = Method.Post,
                        Type = ContentTypes.Approval,
                        Relationship = ActionRelationship.Approve,
                        Reference = $"/timesheets/{UniqueIdentifier}/approval"
                    });

                    break;

                case TimecardStatus.Approved:
                    // terminal state, nothing possible here
                    break;

                case TimecardStatus.Cancelled:
                    links.Add(new ActionLink()
                    {
                        Method = Method.Delete,
                        Type = ContentTypes.Deletion,
                        Relationship = ActionRelationship.Delete,
                        Reference = $"/timesheets/{UniqueIdentifier}/deletion"
                    });
                    break;
            }

            return links;
        }

        private IList<DocumentLink> GetDocumentLinks()
        {
            var links = new List<DocumentLink>();

            links.Add(new DocumentLink()
            {
                Method = Method.Get,
                Type = ContentTypes.Transitions,
                Relationship = DocumentRelationship.Transitions,
                Reference = $"/timesheets/{UniqueIdentifier}/transitions"
            });

            if (this.Lines.Count > 0)
            {
                links.Add(new DocumentLink()
                {
                    Method = Method.Get,
                    Type = ContentTypes.TimesheetLine,
                    Relationship = DocumentRelationship.Lines,
                    Reference = $"/timesheets/{UniqueIdentifier}/lines"
                });
            }

            if (this.Status == TimecardStatus.Submitted)
            {
                links.Add(new DocumentLink()
                {
                    Method = Method.Get,
                    Type = ContentTypes.Transitions,
                    Relationship = DocumentRelationship.Submittal,
                    Reference = $"/timesheets/{UniqueIdentifier}/submittal"
                });
            }

            return links;
        }

        public TimecardLine AddLine(DocumentLine documentLine)
        {
            var annotatedLine = new TimecardLine(documentLine);

            Lines.Add(annotatedLine);

            return annotatedLine;
        }

        // Method to update data of an existing timecard line
        public TimecardLine UpdateLine(DocumentLine documentLine, Guid lineId)
        {
            var annotatedLine = new TimecardLine();
            int lineIndex = FindLineIndex(lineId);
            Lines[lineIndex].Update(documentLine);
            
            return Lines[lineIndex];
        }

        // Method to replace an existing timecard line. The old timecard line will be deleted
        // Since there's no delete line functionality in application. Therefore, I build the delete functionality
        // inside the Repalce method so that other cannot delete a timecard line from outside
        public TimecardLine ReplaceLine(DocumentLine documentLine, Guid lineId)
        {
            var annotatedLine = new TimecardLine(documentLine);
            int lineIndex = FindLineIndex(lineId);
            Lines.RemoveAt(lineIndex);
            Lines.Add(annotatedLine);
            return annotatedLine;
        }

        // Help function to find the index of timecard line in the Line list with the timecard line id.
        private int FindLineIndex(Guid lineId)
        {
            int lineIndex = 0;
            foreach(var line in Lines)
            {
                if(line.UniqueIdentifier == lineId)
                {
                    break;
                }
                lineIndex ++;
            }
            return lineIndex;
        }
        public bool CanBeDeleted()
        {
            return (Status == TimecardStatus.Cancelled || Status == TimecardStatus.Draft);
        }

        public bool HasLine(Guid lineId)
        {
            return Lines
                .Any(l => l.UniqueIdentifier == lineId);
        }


        public override string ToString()
        {
            return PublicJsonSerializer.SerializeObjectIndented(this);
        }
    }
}