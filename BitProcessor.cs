namespace System
{
    enum ABIF_Type
    {
		BYTE=1,
        CHAR=2,
        WORD=3,
        SHORT=4,
        LONG=5,
		RATIONAL=6,
        FLOAT=7,
        DOUBLE=8,
		BCD=9,
        DATE =10,
        TIME=11,
        THUMB = 12,
        BOOL = 13,
		POINT=14,
		RECT=15,
		V_POINT=16,
		V_RECT=17,
        P_STRING =18,
        C_STRING=19,
        TAG=20,
		DELTA_GROUP=128,
		LZW_COMP=256,
		DELTA_LZW=384
    }
    public struct DirEntry
    {
        public int name;
        public string strName;
        public int number;
        public short elementtype;
        public short elementsize;
        public int numelements;
        public int datasize;
        public int dataoffset;
        public int datahandle;
        public override string ToString()
        {
            return string.Format("Name:{0}\r\nNumber:{1}\r\nElement Type:{2}\r\nElement Size:{3}\r\nNum Element:{4}\r\nData Size:{5}\r\nData Offset:{6}\r\nData Handle:{7}\r\n", strName, number, elementtype, elementsize, numelements, datasize, dataoffset, datahandle);
        }
    }
	public struct date
    {
        public int year; // 4-digit year
        public byte month; // month 1-12
        public byte day; // day 1-31
		public DateTime ToDateTime()
        {
            return new DateTime(year, month, day);
        }
		public DateTime AppendTime(time t)
        {
            return new DateTime(year, month, day, t.hour, t.minute, t.second, t.hsecond * 10);
        }
    }
	public struct time
    {
        public byte hour; // hour 0-23
        public byte minute; // minute 0-59
        public byte second; // second 0-59
        public byte hsecond; // 0.01 second 0-99
		public DateTime ToDateTime()
        {
            return new DateTime(1900, 1, 1, hour, minute, second, hsecond * 10);
        }
    }
	public struct thumb
    {
        public int d;
        public int u;
        public byte c;
        public byte n;
    }
    public static partial class BitProcessor_ABIF
    {
        public static DirEntry ToDirEntry(byte[] value, int startindex = 0)
        {
            if (value.Length < 28)
                throw new IndexOutOfRangeException();
            DirEntry res = new DirEntry();
            res.name = BitProcessor.ToInt32(value, 0 + startindex);
            res.strName = BitProcessor.ToString(value, 0 + startindex, 4);
            res.number = BitProcessor.ToInt32(value, 4 + startindex);
            res.elementtype = BitProcessor.ToInt16(value, 8 + startindex);
            res.elementsize = BitProcessor.ToInt16(value, 10 + startindex);
            res.numelements = BitProcessor.ToInt32(value, 12 + startindex);
            res.datasize = BitProcessor.ToInt32(value, 16 + startindex);
            res.dataoffset = BitProcessor.ToInt32(value, 20 + startindex);
            res.datahandle = BitProcessor.ToInt32(value, 24 + startindex);
            return res;
			
        }
		public static date ToDate(byte[] value, int startindex = 0)
        {
			if(value.Length<4)
                throw new IndexOutOfRangeException();
            date res = new date();
            res.year = BitProcessor.ToInt16(value, 0 + startindex);
            res.month = value[2 + startindex];
            res.day = value[3 + startindex];
            return res;
        }
		public static time ToTime(byte[] value, int startindex=0)
        {
            if (value.Length < 8)
                throw new IndexOutOfRangeException();
            time res = new time();
            res.hour = value[0 + startindex];
            res.minute = value[1 + startindex];
            res.second = value[2 + startindex];
            res.hsecond = value[3 + startindex];
            return res;
        }
		public static thumb ToThumb(byte[] value, int startindex)
        {
            if (value.Length < 10)
                throw new IndexOutOfRangeException();
            thumb res = new thumb();
			res.d=BitProcessor.ToInt32(value,0+startindex);
			res.u=BitProcessor.ToInt32(value,4+startindex);
			res.c=value[8+startindex];
			res.n=value[9+startindex];
            return res;
        }
    }
}
