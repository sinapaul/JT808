﻿using JT808.Protocol.Enums;
using JT808.Protocol.Interfaces;
using JT808.Protocol.MessageBody;
using JT808.Protocol.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;
using JT808.Protocol.Formatters;
using JT808.Protocol.MessagePack;
using JT808.Protocol.Attributes;
using JT808.Protocol.Internal;

namespace JT808.Protocol.Test.Simples
{
    public class Demo6
    {
        public JT808Serializer JT808Serializer;
        public Demo6()
        {
            IJT808Config jT808Config = new DefaultGlobalConfig();
            jT808Config.Register(Assembly.GetExecutingAssembly());
            //根据不同的设备终端号，添加自定义消息Id
            jT808Config.MsgIdFactory.CustomSetMap<DT1Demo6>(0x91, "1234567891");
            jT808Config.MsgIdFactory.CustomSetMap<DT2Demo6>(0x91, "1234567892");
            JT808Serializer = new JT808Serializer(jT808Config);
        }

        /// <summary>
        /// 处理多设备多协议消息Id冲突
        /// </summary>
        [Fact]
        public void Test1()
        {
            JT808Package dt1Package = new JT808Package();
            dt1Package.Header = new JT808Header
            {
                MsgId = 0x91,
                MsgNum = 126,
                TerminalPhoneNo = "1234567891"
            };
            DT1Demo6 dT1Demo6 = new DT1Demo6();
            dT1Demo6.Age1 = 18;
            dT1Demo6.Sex1 = 2;
            dt1Package.Bodies = dT1Demo6;

            JT808Package dt2Package = new JT808Package();
            dt2Package.Header = new JT808Header
            {
                MsgId = 0x91,
                MsgNum = 126,
                TerminalPhoneNo = "1234567892"
            };
            DT2Demo6 dT2Demo6 = new DT2Demo6();
            dT2Demo6.Age2 = 18;
            dT2Demo6.Sex2 = 2;
            dt2Package.Bodies = dT2Demo6;
            byte[] dt1Data = JT808Serializer.Serialize(dt1Package);
            var dt1Hex = dt1Data.ToHexString();
            //7E00910003001234567891007D02020012657E
            byte[] dt2Data = JT808Serializer.Serialize(dt2Package);
            var dt2Hex = dt2Data.ToHexString();
            //7E00910003001234567892007D02020012667E
            Assert.Equal("7E00910003001234567891007D02020012657E", dt1Hex);
            Assert.Equal("7E00910003001234567892007D02020012667E", dt2Hex);

            JT808Package dt1Package1 = JT808Serializer.Deserialize(dt1Data);
            Assert.Equal(0x91, dt1Package1.Header.MsgId);
            Assert.Equal(126, dt1Package1.Header.MsgNum);
            Assert.Equal("1234567891", dt1Package1.Header.TerminalPhoneNo);
            DT1Demo6 dt1Bodies = (DT1Demo6)dt1Package1.Bodies;
            Assert.Equal((ushort)18, dt1Bodies.Age1);
            Assert.Equal(2, dt1Bodies.Sex1);

            JT808Package dt2Package1 = JT808Serializer.Deserialize(dt2Data);
            Assert.Equal(0x91, dt2Package1.Header.MsgId);
            Assert.Equal(126, dt2Package1.Header.MsgNum);
            Assert.Equal("1234567892", dt2Package1.Header.TerminalPhoneNo);
            DT2Demo6 dt2Bodies = (DT2Demo6)dt2Package1.Bodies;
            Assert.Equal((ushort)18, dt2Bodies.Age2);
            Assert.Equal(2, dt2Bodies.Sex2);
        }
    }

    [JT808Formatter(typeof(DT1Demo6_Formatter))]
    public class DT1Demo6 : JT808Bodies
    {
        public byte Sex1 { get; set; }

        public ushort Age1 { get; set; }
    }
    [JT808Formatter(typeof(DT2Demo6_Formatter))]
    public class DT2Demo6 : JT808Bodies
    {
        public byte Sex2 { get; set; }

        public ushort Age2 { get; set; }
    }
    public class DT1Demo6_Formatter : IJT808MessagePackFormatter<DT1Demo6>
    {
        public DT1Demo6 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            DT1Demo6 dT1Demo6 = new DT1Demo6();
            dT1Demo6.Sex1 = reader.ReadByte();
            dT1Demo6.Age1 = reader.ReadUInt16();
            return dT1Demo6;
        }

        public void Serialize(ref JT808MessagePackWriter writer, DT1Demo6 value, IJT808Config config)
        {
            writer.WriteByte(value.Sex1);
            writer.WriteUInt16(value.Age1);
        }
    }
    public class DT2Demo6_Formatter : IJT808MessagePackFormatter<DT2Demo6>
    {
        public DT2Demo6 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            DT2Demo6 dT2Demo6 = new DT2Demo6();
            dT2Demo6.Sex2 = reader.ReadByte();
            dT2Demo6.Age2 = reader.ReadUInt16();
            return dT2Demo6;
        }

        public void Serialize(ref JT808MessagePackWriter writer, DT2Demo6 value, IJT808Config config)
        {
            writer.WriteByte(value.Sex2);
            writer.WriteUInt16(value.Age2);
        }
    }
}
