using System;
using System.Runtime.InteropServices;
using ATALoggerLib;
using NUnit.Framework;

namespace ATALoggerLib.Tests
{
    //Tests that mapping corresponds to capture Wireshark messages
    [TestFixture]
    public class MessageSizeTests
    {
        public void GetSerialNumberMessageSizeTest()
        {
            Assert.AreEqual(20, Utils.LenghtOf<GetSerialNumberMessage>());
        }

        public void GetSerialNumberAnswerMessageSizeTest()
        {
            Assert.AreEqual(32+19, Utils.LenghtOf<GetSerialAnswerNumberMessage>());
        }

        public void GetInfo1MessageSizeTest1()
        {
            string SerialNumber="1234567890";
            Assert.AreEqual(20, Messages.GetDataInfo1Message(SerialNumber));
            
        }

        public void GetInfo1MessageSizeTest21()
        {
            string SerialNumber="123456";
            Assert.Exception(Messages.GetDataInfo1Message(SerialNumber));
            
        }

        public void GetInfo1AnswerMessageSizeTest()
        {
            Assert.AreEqual(32+32+12, Utils.LenghtOf<GettInfo1AnswerMessage>());
            
        }
        public void GetInfo2MessageSizeTest()
        {
            Assert.AreEqual(20, Utils.LenghtOf<GettInfo2Message>());
            
        }
        public void GetInfo2AnswerMessageSizeTest()
        {
            Assert.AreEqual(25, Utils.LenghtOf<GetInfo12AnswerMessage>());
            
        }
    }
}