using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Knowledge_checking.Utilities
{
    class ChangingAppearance
    {
        public MainWindow mainWindow;

        public ChangingAppearance(MainWindow mw)
        {
            mainWindow = mw;
        }
        public void ChangeLabelsVisibilityWhenRun(string courseName, string groupName, string testName)
        {     
            mainWindow.courseName.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.groupName.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.testName.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfCourse.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfGroupName.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfTestName.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.getTestButton.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.cancelTestButton.Visibility = System.Windows.Visibility.Visible;
            mainWindow.sendTestButton.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfCourseWhenRun.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfCourseWhenRun.Content = courseName;
            mainWindow.labelOfGroupNameWhenRun.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfGroupNameWhenRun.Content = groupName;
        }

        public void ChangeLabelsVisibilityWhenNotRun()
        {
            mainWindow.courseName.Visibility = System.Windows.Visibility.Visible;
            mainWindow.courseName.Text = "";
            mainWindow.groupName.Visibility = System.Windows.Visibility.Visible;
            mainWindow.groupName.Text = "";
            mainWindow.testName.Visibility = System.Windows.Visibility.Visible;
            mainWindow.testName.Text = "";
            mainWindow.labelOfCourse.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfGroupName.Visibility = System.Windows.Visibility.Visible;
            mainWindow.getTestButton.Visibility = System.Windows.Visibility.Visible;
            mainWindow.cancelTestButton.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.sendTestButton.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfCourseWhenRun.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfGroupNameWhenRun.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ChangeLabelsVisibilityWhenConnected(string ipAdress, string portNumber)
        {
            mainWindow.labelOfPortNumber.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfIpAddress.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.portNumber.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.portNumber.Text = "";
            mainWindow.ipAddress.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.ipAddress.Text = "";
            mainWindow.labelOfIpAddressWhenConnected.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfIpAddressWhenConnected.Content = ipAdress;
            mainWindow.labelOfPortNumberWhenConnected.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfPortNumberWhenConnected.Content = portNumber;
            mainWindow.connectWithClientsButton.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.disconnectWithClientsButton.Visibility = System.Windows.Visibility.Visible;
        }

        public void ChangeLabelsVisibilityWhenDisconnected()
        {
            mainWindow.labelOfPortNumber.Visibility = System.Windows.Visibility.Visible;
            mainWindow.labelOfIpAddress.Visibility = System.Windows.Visibility.Visible;
            mainWindow.portNumber.Visibility = System.Windows.Visibility.Visible;
            mainWindow.portNumber.Text = "";
            mainWindow.ipAddress.Visibility = System.Windows.Visibility.Visible;
            mainWindow.ipAddress.Text = "";
            mainWindow.labelOfIpAddressWhenConnected.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.labelOfPortNumberWhenConnected.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.connectWithClientsButton.Visibility = System.Windows.Visibility.Visible;
            mainWindow.disconnectWithClientsButton.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
