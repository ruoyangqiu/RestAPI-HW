# RestAPI-HW
Ruoyang Qiu

4045856



Remove (DELETE) a draft or cancelled timecard
  
  I add some functionality inside Delete method. And I also add links to switch statement inside GetActionLink() method in Timecard.cs.
  Hence, user can use delete method for draft and canceled time card.

Replace (POST) a complete line item

  The post method to replace timecardline.
  It will replace an existing timecardline completely with input data. It will return the timecardline after updated
  The uniquee id of the original will also be replaced by the new one. 
  And the delete timecard line functionality is implemented inside the support function for replacing in Timecardline.cs. Thus, others
  cannot delete a timecard outside the Timecardline.cs.

Update (PATCH) a line item

  The patch method to update timecardline.
  It will update an existing timecardline with input data and return the timecardline after updated 

Verify that timecard person is consistent throughout the timecard's lifetime

  I add a if/else statement inside post approval method. Hence if approver equals the employee of the submitted timecard, it will return
  an error message.

Verify that timecard approver is not timecard person

  I set the employee's set function to private.
  
Add support to root document for creating a timesheet

   I am not sure if I have a correct understanding of create support.
   I have two opinios. One is just create one below the timesheets with get method. The new one has CreateTimesheets relationship 
   and post as method. 

   Another is pretty complex.
   In my opinion, with create timesheet support state management, User can use it to create as many timesheets as they want.
   I think to implement this create method, the get method need to be rebuild. I need a dictionary to store all timesheets we created.
   And we use a foreach loop to store them in Dictionary and return as a result.
   I implement the code below the Get method.
