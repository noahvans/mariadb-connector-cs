// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation; version 3 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
//
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Data;
using System.Globalization;
using MariaDB.Data.MySqlClient;

namespace MariaDB.Data.Types
{
    internal struct MySqlByte : IMySqlValue
    {
        private sbyte mValue;
        private bool isNull;
        private bool treatAsBool;

        public MySqlByte(bool isNull)
        {
            this.isNull = isNull;
            mValue = 0;
            treatAsBool = false;
        }

        public MySqlByte(sbyte val)
        {
            this.isNull = false;
            mValue = val;
            treatAsBool = false;
        }

        public bool IsNull
        {
            get { return isNull; }
        }

        MySqlDbType IMySqlValue.MySqlDbType
        {
            get { return MySqlDbType.Byte; }
        }

        DbType IMySqlValue.DbType
        {
            get
            {
                if (TreatAsBoolean)
                    return DbType.Boolean;
                return DbType.SByte;
            }
        }

        object IMySqlValue.Value
        {
            get
            {
                if (TreatAsBoolean)
                    return Convert.ToBoolean(mValue);
                return mValue;
            }
        }

        public sbyte Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        Type IMySqlValue.SystemType
        {
            get
            {
                if (TreatAsBoolean)
                    return typeof(Boolean);
                return typeof(sbyte);
            }
        }

        string IMySqlValue.MySqlTypeName
        {
            get { return "TINYINT"; }
        }

        void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
        {
            sbyte v = (val is sbyte) ? (sbyte)val : Convert.ToSByte(val);
            if (binary)
                packet.WriteByte((byte)v);
            else
                packet.WriteStringNoNull(v.ToString());
        }

        IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
        {
            if (nullVal)
                return new MySqlByte(true);

            if (length == -1)
                return new MySqlByte((sbyte)packet.ReadByte());
            else
            {
                string s = packet.ReadString(length);
                MySqlByte b = new MySqlByte(SByte.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture));
                b.TreatAsBoolean = TreatAsBoolean;
                return b;
            }
        }

        void IMySqlValue.SkipValue(MySqlPacket packet)
        {
            packet.ReadByte();
        }

        internal bool TreatAsBoolean
        {
            get { return treatAsBool; }
            set { treatAsBool = value; }
        }

        internal static void SetDSInfo(DataTable dsTable)
        {
            // we use name indexing because this method will only be called
            // when GetSchema is called for the DataSourceInformation
            // collection and then it will be cached.
            DataRow row = dsTable.NewRow();
            row["TypeName"] = "TINYINT";
            row["ProviderDbType"] = MySqlDbType.Byte;
            row["ColumnSize"] = 0;
            row["CreateFormat"] = "TINYINT";
            row["CreateParameters"] = null;
            row["DataType"] = "System.SByte";
            row["IsAutoincrementable"] = true;
            row["IsBestMatch"] = true;
            row["IsCaseSensitive"] = false;
            row["IsFixedLength"] = true;
            row["IsFixedPrecisionScale"] = true;
            row["IsLong"] = false;
            row["IsNullable"] = true;
            row["IsSearchable"] = true;
            row["IsSearchableWithLike"] = false;
            row["IsUnsigned"] = false;
            row["MaximumScale"] = 0;
            row["MinimumScale"] = 0;
            row["IsConcurrencyType"] = DBNull.Value;
            row["IsLiteralSupported"] = false;
            row["LiteralPrefix"] = null;
            row["LiteralSuffix"] = null;
            row["NativeDataType"] = null;
            dsTable.Rows.Add(row);
        }
    }
}