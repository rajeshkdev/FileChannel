using B360.Notifier.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace FileNotificationChannel
{
    // Note: "System.Runtime.Serialization" -- This Assembly need to be added as a reference to this Project.
    public class FileChannel : IChannelNotification
    {
        public string GetGlobalPropertiesSchema()
        {
            return Helper.GetResourceFileContent("GlobalProperties.xml");
        }

        public string GetAlarmPropertiesSchema()
        {
            return Helper.GetResourceFileContent("AlarmProperties.xml");
        }

        public bool SendNotification(BizTalkEnvironment environment, Alarm alarm, string globalProperties, Dictionary<MonitorGroupTypeName, MonitorGroupData> notifications)
        {
            //Construct Message
            string message = string.Empty;
            message += String.Format("\nAlarm Name: {0} \n\nAlarm Desc: {1} \n", alarm.Name, alarm.Description);
            message += "\n----------------------------------------------------------------------------------------------------\n";
            message += String.Format("\nEnvironment Name: {0} \n\nMgmt Sql Instance Name: {1} \nMgmt Sql Db Name: {2}\n", environment.Name, environment.MgmtSqlDbName, environment.MgmtSqlInstanceName);
            message += "\n----------------------------------------------------------------------------------------------------\n";

            // Change the Message Type and Format based on your need.
            BT360Helper helper = new BT360Helper(notifications, environment, alarm, MessageType.ConsolidatedMessage, MessageFormat.Text); 
            message += helper.GetNotificationMessage();

            //Read configured properties
            XNamespace bsd = XNamespace.Get("http://www.biztalk360.com/alarms/notification/basetypes");
            XNamespace prop = XNamespace.Get("http://www.biztalk360.com/alarms/notification/properties");
            XNamespace xmlns = XNamespace.Get("http://www.biztalk360.com/alarms/notification/basetypes");

            string fileFolder = string.Empty; string folderOverride = string.Empty;

            //Alarm Properties
            XDocument almProps = XDocument.Load(new StringReader(alarm.AlarmProperties));
            foreach (var element in almProps.Descendants(bsd + "TextBox"))
            {
                if (element.Attribute("Name").Value == "file-override-path")
                    folderOverride = element.Attribute("Value").Value;
            }

            //Global Properties
            XDocument globalProps = XDocument.Load(new StringReader(globalProperties));
            foreach (var element in globalProps.Descendants(bsd + "TextBox"))
            {
                if (element.Attribute("Name").Value == "file-path")
                    fileFolder = element.Attribute("Value").Value;
            }

            //Save to Disk
            string filePath = string.IsNullOrEmpty(folderOverride) ? fileFolder : folderOverride;
            string fileLocation = Path.Combine(filePath, Guid.NewGuid().ToString(), ".txt");
            using (FileStream fs = new FileStream(fileLocation, FileMode.CreateNew))
            {
                fs.Write(Encoding.UTF8.GetBytes(message), 0, message.Length);
            }

            return true;
        }
    }
}
