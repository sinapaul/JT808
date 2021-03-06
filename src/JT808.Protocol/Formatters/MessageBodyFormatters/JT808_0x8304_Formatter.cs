﻿using JT808.Protocol.Extensions;
using JT808.Protocol.MessageBody;
using JT808.Protocol.Interfaces;
using System;
using JT808.Protocol.MessagePack;

namespace JT808.Protocol.Formatters.MessageBodyFormatters
{
    public class JT808_0x8304_Formatter : IJT808MessagePackFormatter<JT808_0x8304>
    {
        public JT808_0x8304 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            JT808_0x8304 jT808_0X8304 = new JT808_0x8304();
            jT808_0X8304.InformationType = reader.ReadByte();
            jT808_0X8304.InformationLength = reader.ReadUInt16();
            jT808_0X8304.InformationContent = reader.ReadString(jT808_0X8304.InformationLength);
            return jT808_0X8304;
        }

        public void Serialize(ref JT808MessagePackWriter writer, JT808_0x8304 value, IJT808Config config)
        {
            writer.WriteByte(value.InformationType);
            // 先计算内容长度（汉字为两个字节）
            writer.Skip(2, out int position);
            writer.WriteString(value.InformationContent);
            ushort length =(ushort)( writer.GetCurrentPosition() - position - 2);
            writer.WriteUInt16Return(length, position);
        }
    }
}
