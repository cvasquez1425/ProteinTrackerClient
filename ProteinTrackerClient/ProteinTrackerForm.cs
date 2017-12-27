#region Includes
using ProteinTrackerClient.ProteinTrackerService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#endregion

namespace ProteinTrackerClient
{
    public partial class ProteinTrackerForm : Form
    {
        // Create an instance of this service. Member variable to hold this. service is our PROXY CLASS (i.e., .NET class, local .NET object), we are treating not like a local object, but like it is making service calls.
        private ProteinTrackingServiceSoapClient service = new ProteinTrackingServiceSoapClient();

        private User[] users; //because it's going to be deserialized by default as an array.

        public ProteinTrackerForm()
        {
            InitializeComponent();
        }

        // we are going to call our service and store the list of users
        private void OnLoad(object sender, EventArgs e)
        {
            users = service.ListUsers();                // it'll give you our list of users and you can see that this returns back that array of users. This is actually a proxy method on this proxy class, and it's going to go out and do the work of calling Web Service for us.
            cboSelectUser.DataSource    = users;
            cboSelectUser.DisplayMember = "Name";
            cboSelectUser.ValueMember   = "UserId";
        }

        private void OnAddNewUser(object sender, EventArgs e)
        {
            // we wanna call our web service method of AddUser
            service.AddUser(txtUserName.Text, int.Parse(txtUserGoal.Text));
            users = service.ListUsers();
            cboSelectUser.DataSource = users;
        }

        private void OnUserChanged(object sender, EventArgs e)
        {
            // Add some functionalities here.
            var index       = cboSelectUser.SelectedIndex;
            lblTotal.Text   = users[index].Total.ToString();
            lblGoal.Text    = users[index].Goal.ToString();
        }

        // async await patter, meaning our call go ahead and execute without blocking, so we won't block the UI thread.
        async private void OnAddProtein(object sender, EventArgs e)
        {
            // Try/Catch to handle exceptions on the client.
            var userId                                  = users[cboSelectUser.SelectedIndex].UserId; // it'll give you our user object, and then we'll have our UserId
            try
            {
                //var auth = new AuthenticationHeader { UserName = "Bob", Password = "Pass" };
                var newTotal = await service.AddProteinAsync(int.Parse(txtAmount.Text), userId);    // allows the call to web service to be made asysncronously, and it's NOT going to block the UI thread. it won't freeze up the entire UI. avoid the blocking. But go ahead and exit this method, we're done, keep the UI going, so we can do other things.
                users[cboSelectUser.SelectedIndex].Total = newTotal;                                // pause and wait to execute the rest of this code until this call finishes. I am not Frozen.
                lblTotal.Text = newTotal.ToString();
            }
            catch (FaultException exception)                                                        // all the Exceptions that come across are basically going to be wrapped in this FaultException, which is SOAP fault.S
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
