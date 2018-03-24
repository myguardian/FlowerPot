using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiConnect.BusinessLogic
{
    class Alert
    {
        private int _alertID;
        private string _flowerpotID;
        private DateTime _alertDateTime;
        private string _shortDescription;
        private string _longDescription;
        private DateTime _acknowledgeDateTime;
        private int _alertLevel;

        //TODO: Fields for image and sound
        private string _image;
        private string _sound;

        public Alert(int alertID, string fpID, DateTime alertDT, string shortDesc, string longDesc, int alertLvl)
        {
            _alertID = alertID;
            _flowerpotID = fpID;
            _alertDateTime = alertDT;
            _shortDescription = shortDesc;
            _longDescription = longDesc;

            _acknowledgeDateTime = DateTime.Parse("1001-01-01 00:00:00");

            _alertLevel = alertLvl;

            //TODO: Temporary
            _image = null;
            _sound = null;
        }

        public int AlertID
        {
            get { return _alertID; }
            set { _alertID = value; }
        }
        public string FlowerpotID
        {
            get { return _flowerpotID; }
            set { _flowerpotID = value; }
        }
        public DateTime AlertDateTime
        {
            get { return _alertDateTime; }
            set { _alertDateTime = value; }
        }
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set { _longDescription = value; }
        }
        public DateTime AcknowledgeDateTime
        {
            get { return _acknowledgeDateTime; }
            set { _acknowledgeDateTime = value; }
        }
        public int AlertLevel
        {
            get { return _alertLevel; }
            set { _alertLevel = value; }
        }

        public string toString()
        {
            return _shortDescription;
        }
    }
}
