using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using restapi.Models;

namespace restapi.Controllers
{
    public class RootController : Controller
    {
        // GET api/values
        
        [Route("~/")]
        [HttpGet]
        [Produces(ContentTypes.Root)]
        [ProducesResponseType(typeof(IDictionary<ApplicationRelationship, object>), 200)]
        public IDictionary<ApplicationRelationship, object> Get()
        {
            return new Dictionary<ApplicationRelationship, object>()
            {
                {
                    ApplicationRelationship.Timesheets, new List<DocumentLink>()
                    {
                        new DocumentLink()
                        {
                            Method = Method.Get,
                            Type = ContentTypes.Timesheets,
                            Relationship = DocumentRelationship.Timesheets,
                            Reference = "/timesheets"
                        }
                    }
                    
                },

                /*
                I am not sure if I have a correct understanding of create support.
                I have two opinios. One is just create one below the timesheets with get method. The new one
                has CreateTimesheets relationship and post as method. 

                Another is pretty complex.
                In my opinion, with create timesheet support state management, User can use it to create as many timesheets as they want.
                I think to implement this create method, the get method need to be rebuild. I need a dictionary to store all timesheets we created.
                And we use a foreach loop to store them in Dictionary and return as a result.
                I implement the code below the Get method.   
                */
                {
                    ApplicationRelationship.CreateTimesheets, new List<DocumentLink>()
                    {
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Timesheets,
                            Relationship = DocumentRelationship.CreateTimesheet,
                            Reference = "/timesheets"
                        }
                    }
                },
                {
                    ApplicationRelationship.Version, "0.1"
                }
            };
        }

    

        // This is new get and create for creating new timesheet as many as a user want
        // 
        /* 

        private readonly IDictionary<ApplicationRelationship, object> dict = new Dictionary<ApplicationRelationship, object>(){{ApplicationRelationship.Version, "0.1"}};
        [Route("~/")]
        [HttpGet]
        [Produces(ContentTypes.Root)]
        [ProducesResponseType(typeof(IDictionary<ApplicationRelationship, object>), 200)]
        public IDictionary<ApplicationRelationship, object> Get()
        {
            return dict;
        }

        
        [Route("~/")]
        [HttpPost]
        [Produces(ContentTypes.Root)]
        [ProducesResponseType(typeof(IDictionary<ApplicationRelationship, object>), 200)]
        public IDictionary<ApplicationRelationship, object> Creat()
        {
            dict.Add(ApplicationRelationship.Timesheets, new List<DocumentLink>()
                    {
                        new DocumentLink()
                        {
                            Method = Method.Post,
                            Type = ContentTypes.Timesheets,
                            Relationship = DocumentRelationship.CreateTimesheet,
                            Reference = "/timesheets"
                        }
                    });
        } */
    }
}
