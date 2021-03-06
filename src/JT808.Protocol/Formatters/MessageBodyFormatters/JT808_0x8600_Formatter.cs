﻿using JT808.Protocol.Extensions;
using JT808.Protocol.Metadata;
using JT808.Protocol.MessageBody;
using JT808.Protocol.Interfaces;
using System;
using System.Collections.Generic;
using JT808.Protocol.MessagePack;

namespace JT808.Protocol.Formatters.MessageBodyFormatters
{
    public class JT808_0x8600_Formatter : IJT808MessagePackFormatter<JT808_0x8600>
    {
        public JT808_0x8600 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            JT808_0x8600 jT808_0X8600 = new JT808_0x8600();
            jT808_0X8600.SettingAreaProperty = reader.ReadByte();
            jT808_0X8600.AreaCount = reader.ReadByte();
            jT808_0X8600.AreaItems = new List<JT808CircleAreaProperty>();
            for (var i = 0; i < jT808_0X8600.AreaCount; i++)
            {
                JT808CircleAreaProperty jT808CircleAreaProperty = new JT808CircleAreaProperty();
                jT808CircleAreaProperty.AreaId = reader.ReadUInt32();
                jT808CircleAreaProperty.AreaProperty = reader.ReadUInt16();
                jT808CircleAreaProperty.CenterPointLat = reader.ReadUInt32();
                jT808CircleAreaProperty.CenterPointLng = reader.ReadUInt32();
                jT808CircleAreaProperty.Radius = reader.ReadUInt32();
                ReadOnlySpan<char> areaProperty16Bit = Convert.ToString(jT808CircleAreaProperty.AreaProperty, 2).PadLeft(16, '0').AsSpan();
                bool bit0Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 1).ToString().Equals("0");
                if (!bit0Flag)
                {
                    jT808CircleAreaProperty.StartTime = reader.ReadDateTime6();
                    jT808CircleAreaProperty.EndTime = reader.ReadDateTime6();
                }
                bool bit1Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 2, 1).ToString().Equals("0");
                if (!bit1Flag)
                {
                    jT808CircleAreaProperty.HighestSpeed = reader.ReadUInt16();
                    jT808CircleAreaProperty.OverspeedDuration = reader.ReadByte();
                }
                jT808_0X8600.AreaItems.Add(jT808CircleAreaProperty);
            }
            return jT808_0X8600;
        }

        public void Serialize(ref JT808MessagePackWriter writer, JT808_0x8600 value, IJT808Config config)
        {
            writer.WriteByte(value.SettingAreaProperty);
            if (value.AreaItems != null)
            {
                writer.WriteByte((byte)value.AreaItems.Count);
                foreach (var item in value.AreaItems)
                {
                    writer.WriteUInt32(item.AreaId);
                    writer.WriteUInt16(item.AreaProperty);
                    writer.WriteUInt32(item.CenterPointLat);
                    writer.WriteUInt32(item.CenterPointLng);
                    writer.WriteUInt32(item.Radius);
                    ReadOnlySpan<char> areaProperty16Bit = Convert.ToString(item.AreaProperty, 2).PadLeft(16, '0').AsSpan();
                    bool bit0Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 1).ToString().Equals("0");
                    if (!bit0Flag)
                    {
                        if (item.StartTime.HasValue)
                        {
                            writer.WriteDateTime6(item.StartTime.Value);
                        }
                        if (item.EndTime.HasValue)
                        {
                            writer.WriteDateTime6(item.EndTime.Value);
                        }
                    }
                    bool bit1Flag = areaProperty16Bit.Slice(areaProperty16Bit.Length - 2, 1).ToString().Equals("0");
                    if (!bit1Flag)
                    {
                        if (item.HighestSpeed.HasValue)
                        {
                            writer.WriteUInt16(item.HighestSpeed.Value);
                        }
                        if (item.OverspeedDuration.HasValue)
                        {
                            writer.WriteByte(item.OverspeedDuration.Value);
                        }
                    }
                }
            }
        }
    }
}
