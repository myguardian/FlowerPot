using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiConnect
{
    class User
    {
        private string _flowerpotID;
        private string _tagSerialNum;
        private string _firstName;
        private string _lastName;
        private string _emailAddress;
        private int _age;
        private char _gender;
        private long _phoneNumber;

        public User(string fpID, string tagSerNum, string fName, string lName, string emailAddr, 
            int age, char gender, long pNumber)
        {
            _flowerpotID = fpID;
            _tagSerialNum = tagSerNum;
            _firstName = fName;
            _lastName = lName;
            _emailAddress = emailAddr;
            _age = age;
            _gender = gender;
            _phoneNumber = pNumber;
        }

        public string FlowerPotID
        {
            get { return _flowerpotID; }
            set { _flowerpotID = value; }
        }
        public string TagSerialNum
        {
            get { return _tagSerialNum; }
            set { _tagSerialNum = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
        public char Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }
        public long PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }
    }
}
