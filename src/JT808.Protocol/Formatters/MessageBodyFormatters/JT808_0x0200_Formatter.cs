﻿using JT808.Protocol.Extensions;
using JT808.Protocol.Metadata;
using JT808.Protocol.MessageBody;
using System;
using System.Collections.Generic;
using JT808.Protocol.Interfaces;
using JT808.Protocol.MessagePack;
using JT808.Protocol.Exceptions;
using JT808.Protocol.Enums;

namespace JT808.Protocol.Formatters.MessageBodyFormatters
{
    public class JT808_0x0200_Formatter : IJT808MessagePackFormatter<JT808_0x0200>
    {
        public JT808_0x0200 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            JT808_0x0200 jT808_0X0200 = new JT808_0x0200();
            jT808_0X0200.AlarmFlag = reader.ReadUInt32();
            jT808_0X0200.StatusFlag = reader.ReadUInt32();
            if (((jT808_0X0200.StatusFlag >> 28) & 1) == 1)
            {   //南纬 268435456 0x10000000
                jT808_0X0200.Lat = (int)reader.ReadUInt32();
            }
            else
            {
                jT808_0X0200.Lat = reader.ReadInt32();
            }
            if (((jT808_0X0200.StatusFlag >> 27) & 1) == 1)
            {   //西经 ‭134217728‬ 0x8000000
                jT808_0X0200.Lng = (int)reader.ReadUInt32();
            }
            else
            {
                jT808_0X0200.Lng = reader.ReadInt32();
            }
            jT808_0X0200.Altitude = reader.ReadUInt16();
            jT808_0X0200.Speed = reader.ReadUInt16();
            jT808_0X0200.Direction = reader.ReadUInt16();
            jT808_0X0200.GPSTime = reader.ReadDateTime6();
            // 位置附加信息
            jT808_0X0200.JT808LocationAttachData = new Dictionary<byte, JT808_0x0200_BodyBase>();
            jT808_0X0200.JT808CustomLocationAttachOriginalData = new Dictionary<byte, byte[]>();
            jT808_0X0200.JT808UnknownLocationAttachOriginalData = new Dictionary<byte, byte[]>();
            while (reader.ReadCurrentRemainContentLength()>0)
            {
                try
                {
                    ReadOnlySpan<byte> attachSpan= reader.GetVirtualReadOnlySpan(2);
                    byte attachId = attachSpan[0];
                    byte attachLen = attachSpan[1];
                    if (config.JT808_0X0200_Factory.JT808LocationAttachMethod.TryGetValue(attachId, out Type jT808LocationAttachType))
                    {
                        object attachImplObj = config.GetMessagePackFormatterByType(jT808LocationAttachType);
                        dynamic attachImpl = JT808MessagePackFormatterResolverExtensions.JT808DynamicDeserialize(attachImplObj,ref reader, config);
                        jT808_0X0200.JT808LocationAttachData.Add(attachImpl.AttachInfoId, attachImpl);
                    }
                    else if (config.JT808_0X0200_Custom_Factory.AttachIds.Contains(attachId))
                    {
                        reader.Skip(2);
                        jT808_0X0200.JT808CustomLocationAttachOriginalData.Add(attachId, reader.ReadArray(reader.ReaderCount-2, attachLen+2).ToArray());
                        reader.Skip(attachLen);
                    }
                    else
                    {
                        reader.Skip(2);
                        jT808_0X0200.JT808UnknownLocationAttachOriginalData.Add(attachId, reader.ReadArray(reader.ReaderCount-2, attachLen+2).ToArray());
                        reader.Skip(attachLen);
                    }
                }
                catch
                {
                    try
                    {
                        byte attachId = reader.ReadByte();
                        byte attachLen = reader.ReadByte();
                        jT808_0X0200.JT808UnknownLocationAttachOriginalData.Add(attachId, reader.ReadArray(reader.ReaderCount - 2, attachLen+2).ToArray());
                        reader.Skip(attachLen);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return jT808_0X0200;
        }

        public void Serialize(ref JT808MessagePackWriter writer, JT808_0x0200 value, IJT808Config config)
        {
            writer.WriteUInt32(value.AlarmFlag);
            writer.WriteUInt32(value.StatusFlag);
            //0x10000000 南纬 134217728
            //0x8000000  西经 ‭‬268435456
            //0x18000000 南纬-西经 134217728+268435456
            if (((value.StatusFlag >> 28) & 1) == 1)
            {
                uint lat=(uint)value.Lat;
                writer.WriteUInt32(lat);
            }
            else
            {
                if (value.Lat < 0)
                {
                    throw new JT808Exception(JT808ErrorCode.LatOrLngError, $"Lat {nameof(JT808_0x0200.StatusFlag)} ({value.StatusFlag}>>28) !=1");
                }
                writer.WriteInt32(value.Lat);
            }
            if (((value.StatusFlag >> 27) & 1) == 1)
            {
                uint lng = (uint)value.Lng;
                writer.WriteUInt32(lng);
            }
            else
            {
                if (value.Lng < 0)
                {
                    throw new JT808Exception(JT808ErrorCode.LatOrLngError, $"Lng {nameof(JT808_0x0200.StatusFlag)} ({value.StatusFlag}>>29) !=1");
                }
                writer.WriteInt32(value.Lng);
            }
            writer.WriteUInt16(value.Altitude);
            writer.WriteUInt16(value.Speed);
            writer.WriteUInt16(value.Direction);
            writer.WriteDateTime6(value.GPSTime);
            if (value.JT808LocationAttachData != null && value.JT808LocationAttachData.Count > 0)
            {
                foreach (var item in value.JT808LocationAttachData)
                {
                    try
                    {
                        object attachImplObj = config.GetMessagePackFormatterByType(item.Value.GetType());
                        JT808MessagePackFormatterResolverExtensions.JT808DynamicSerialize(attachImplObj, ref writer,item.Value, config);
                    }
                    catch
                    {

                    }
                }
            }
            if (value.JT808CustomLocationAttachData != null && value.JT808CustomLocationAttachData.Count > 0)
            {
                foreach (var item in value.JT808CustomLocationAttachData)
                {
                    object attachImplObj = config.GetMessagePackFormatterByType(item.Value.GetType());
                    JT808MessagePackFormatterResolverExtensions.JT808DynamicSerialize(attachImplObj, ref writer, item.Value, config);
                }
            }
        }
    }
}
