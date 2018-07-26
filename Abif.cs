using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.BitProcessor;


namespace AbifInterface
{
    #region 基础数据类型
    /// <summary>
    /// 峰图数据载体
    /// </summary>
    public partial class BaseData
    {
        /// <param name="Index">数据的索引</param>
        /// <returns></returns>
        public BaseCell this[int Index]
        {
            get
            {
                return DataList[Index];
            }
        }
        public int Count { get { return DataList.Count; } }
            
        protected List<BaseCell> DataList=new List<BaseCell>();
        /// <summary>
        /// 请勿使用此方法
        /// </summary>
        /// <param name="data">请勿使用此方法</param>
        public void PushBack(BaseCell data)
        {
            DataList.Add(data);
        }
        public override string ToString()
        {
            string res="";
            res += "A ";
            for(int i=0;i<DataList.Count;i++)
                res += DataList[i][BaseType.A] + " ";
            res += "\r\nT ";
            for (int i = 0; i < DataList.Count; i++)
                res += DataList[i][BaseType.T] + " ";
            res += "\r\nC ";
            for (int i = 0; i < DataList.Count; i++)
                res += DataList[i][BaseType.C] + " ";
            res += "\r\nG ";
            for (int i = 0; i < DataList.Count; i++)
                res += DataList[i][BaseType.G] + " ";
            return res;
        }

    }
    /// <summary>
    /// 单个位点的数据
    /// </summary>
    public class BaseCell
    {

        protected int[] dat;
        protected int offset;
        public BaseCell(int A, int T, int C, int G, int Offset)
        {
            dat = new int[4]{ A, T, C, G};
            offset = Offset;
        }

        /// <param name="bd">碱基类型
        /// <code>
        /// enum BaseType
        /// { A, T, C, G }
        /// </code>
        /// </param>
        /// <returns>该点处相应碱基的电压值</returns>
        public int this[BaseType bd]
        {
            get
            {
                return dat[(int)bd];
            }
        }

        /// <summary>
        /// 位点的偏移量
        /// </summary>
        public int Offset { get { return offset; } }
    }
    /// <summary>
    /// ABIF错误
    /// </summary>
    public class ABIFException: SystemException
    {
        //
        // 摘要:
        //     初始化 System.IndexOutOfRangeException 类的新实例。
        public ABIFException() : base() { }
        //
        // 摘要:
        //     使用指定的错误信息初始化 System.IndexOutOfRangeException 类的新实例。
        //
        // 参数:
        //   message:
        //     描述错误的消息。
        public ABIFException(string message) : base(message) { }
        //
        // 摘要:
        //     使用指定错误信息和对作为此异常原因的内部异常的引用来初始化 System.IndexOutOfRangeException 类的新实例。
        //
        // 参数:
        //   message:
        //     解释异常原因的错误信息。
        //
        //   innerException:
        //     导致当前异常的异常。如果 innerException 参数不是空引用（在 Visual Basic 中为 Nothing），则在处理内部异常的 catch
        //     块中引发当前异常。
        public ABIFException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion
    public class Abif : IAbif
    {
        //Vars
        protected BaseData data=new BaseData(), seq=new BaseData();
        protected List<DirEntry> DirList=new List<DirEntry>();
        protected int DyeNum;
        protected string[] DyeName = new string[4];
        protected int[] DyeWaveLength = new int [4];
        protected string FileName;
        /// <summary>
        /// 序列原始数据数据
        /// </summary>
        public BaseData Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// 序列峰值数据
        /// </summary>
        public BaseData Seq
        {
            get
            {
                return seq;
            }
        }
        protected byte[] bBuffer;
        //private functions
        bool CmpName(DirEntry dir,string name)
        {
            return (dir.name == ToInt32(Encoding.Default.GetBytes(name), 0));
        }

        byte[] GetData(DirEntry dir)
        {
            byte[] res;
            if (dir.datasize <= 4)
                return BitProcessor.GetBytes(dir.dataoffset);
            else
            {
                res = new byte[dir.datasize];
                for (int i = 0; i < dir.datasize; i++)
                    res[i] = bBuffer[dir.dataoffset + i];
                return res;
            }
        }
        void PrintData(DirEntry dir)
        {
            byte[] data = GetData(dir);
            switch((ABIF_Type)dir.elementtype)
            {
                case ABIF_Type.BOOL:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(ToBoolean(data, i));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.BYTE:
                    Console.WriteLine(data);
                    break;
                case ABIF_Type.CHAR:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(Encoding.ASCII.GetChars(data, 0,dir.numelements));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.FLOAT:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(BitConverter.ToSingle(data, i));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.SHORT:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(ToInt16(data, i));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.LONG:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(ToInt32(data, i));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.WORD:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        Console.Write(ToInt32(data, i));
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                    break;
                case ABIF_Type.P_STRING:
                    Console.WriteLine(BitProcessor.ToString(data, 1, data[0]));
                    break;
                case ABIF_Type.C_STRING:
                    Console.WriteLine(BitProcessor.ToString(data, 0, dir.datasize));
                    break;
                default:
                    Console.WriteLine(data);
                    break;
            }
        
        }
        void PrintDataToFile(DirEntry dir)
        {
            string content=dir.ToString();
            byte[] data = GetData(dir);
            switch ((ABIF_Type)dir.elementtype)
            {
                case ABIF_Type.BOOL:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        content+=(ToBoolean(data, i));
                        content+=(" ");
                    }
                    content+= "\r\n";
                    break;
                case ABIF_Type.BYTE:
                    content+=(BitConverter.ToString(data));
                    content += "\r\n";
                    break;
                case ABIF_Type.CHAR:
                    content+=new string(Encoding.ASCII.GetChars(data, 0, dir.numelements));
                    content+=(" ");
                    content+= "\r\n";
                    break;
                case ABIF_Type.FLOAT:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        content+=(BitConverter.ToSingle(data, i));
                        content+=(" ");
                    }
                    content+= "\r\n";
                    break;
                case ABIF_Type.SHORT:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        content+=(ToInt16(data, i));
                        content+=(" ");
                    }
                    content+= "\r\n";
                    break;
                case ABIF_Type.LONG:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        content+=(ToInt32(data, i));
                        content+=(" ");
                    }
                    content+= "\r\n";
                    break;
                case ABIF_Type.WORD:
                    for (int i = 0; i < dir.numelements; i++)
                    {
                        content+=(ToInt32(data, i));
                        content+=(" ");
                    }
                    content+= "\r\n";
                    break;
                case ABIF_Type.P_STRING:
                    content+=(BitProcessor.ToString(data, 1, data[0]));
                    break;
                case ABIF_Type.C_STRING:
                    content+=(BitProcessor.ToString(data, 0, dir.datasize));
                    break;
                default:
                    content+=BitConverter.ToString(data);
                    content += "\r\n";
                    break;
            }
            Directory.CreateDirectory(".\\Data\\" + FileName + "\\");
            File.WriteAllText(".\\Data\\" + FileName + "\\" + dir.strName.Replace('/', '_') + dir.number + ".txt", content);
            
        }
        void PrintDataToFile(BaseData data)
        {
            Directory.CreateDirectory(".\\Data\\" + FileName + "\\");
            File.WriteAllText(".\\Data\\" + FileName + "\\" + "BaseData.txt", data.ToString());
        }
        int Compare(BaseCell c1, BaseCell c2)
        {
            int t1, t2;
            t1 = c1[BaseType.A];
            t2 = c2[BaseType.A];
            if(c1[BaseType.C]>c1[BaseType.A])
            {
                t1 = c1[BaseType.C];
                t2 = c2[BaseType.C];
            }
            if(c1[BaseType.G]>c1[BaseType.G])
            {
                t1 = c1[BaseType.G];
                t2 = c2[BaseType.G];
            }
            if (c1[BaseType.T] > c1[BaseType.T])
            {
                t1 = c1[BaseType.T];
                t2 = c2[BaseType.T];
            }
            if (t1 > t2) return 1;
            else if (t1 < t2) return -1;
            else return 1;
        }
        //public functions
        /// <summary>
        /// 读取ab1文件
        /// </summary>
        /// <param name="sFileName">（文件路径和）文件名</param>
        public void ReadFile(string sFileName)
        {
            #region 读取文件
            FileName = sFileName;
            //throw new NotImplementedException();
            FileStream fs = new FileStream(sFileName, FileMode.Open);
            BinaryReader binReader = new BinaryReader(fs);
            bBuffer = new byte[fs.Length];
            binReader.Read(bBuffer, 0, (int)fs.Length);
            binReader.Close();
            fs.Close();
            #endregion
            /*-------------------------------------------------------------------------------------*/
            #region 判断文件合法性
            string str = new string(Encoding.ASCII.GetChars(bBuffer, 0, 4));
            if (str != "ABIF")
                throw new ABIFException("Not an ABIF file");
            int ver = ToInt16(bBuffer, 4);
            if (ver/100!=1)
                throw new ABIFException("ABIF version not supported");
            DirList.Add(BitProcessor_ABIF.ToDirEntry(bBuffer, 6));
            for(int i=0;i<DirList[0].numelements;i++)
                DirList.Add(BitProcessor_ABIF.ToDirEntry(bBuffer, DirList[0].dataoffset + i * 28));
            foreach (DirEntry dir in DirList)
                if (CmpName(dir, "Dye#"))
                {
                    DyeNum = dir.dataoffset;
                    break;
                }
            #region 调试
            //int j = 0;
            //foreach (DirEntry dir in DirList)
            //{
            //    //Console.WriteLine(j);
            //    //Console.WriteLine(dir.ToString());
            //    //PrintDataToFile(dir);
            //    j++;
            //}
            #endregion
#endregion
            /*-------------------------------------------------------------------------------------*/
            #region 制作原始数据
            List<int> A=new List<int>(), T=new List<int>(), C=new List<int>(), G=new List<int>();
            foreach (DirEntry dir in DirList)
                if(CmpName(dir,"DATA"))
                {
                    switch (dir.number)
                    {
                        case 9:
                            for (int i = 0; i < dir.numelements; i++)
                                G.Add(ToInt16(bBuffer, dir.dataoffset + 2 * i));
                            break;
                        case 10:
                            for (int i = 0; i < dir.numelements; i++)
                                A.Add(ToInt16(bBuffer, dir.dataoffset + 2 * i));
                            break;
                        case 11:
                            for (int i = 0; i < dir.numelements; i++)
                                T.Add(ToInt16(bBuffer, dir.dataoffset + 2 * i));
                            break;
                        case 12:
                            for (int i = 0; i < dir.numelements; i++)
                                C.Add(ToInt16(bBuffer, dir.dataoffset + 2 * i));
                            break;
                    }
                }
            for (int i = 0; i < A.Count; i+=2)
                data.PushBack(new BaseCell(A[i], T[i], C[i], G[i], i));
            #region 调试
            //Console.WriteLine(data.ToString());
            //PrintDataToFile(data);
            #endregion
            #endregion
            /*-------------------------------------------------------------------------------------*/
            #region 制作峰值数据
            //峰位置为区间上局部最大值的稳定值
            //多峰的offset取平均值。多峰的判定标准为？
            for(int i=1;i<data.Count-1;i++)
                if ((Compare(data[i], data[i + 1]) + Compare(data[i - 1], data[i]) == 0) && Compare(data[i], new BaseCell(150,150,150,150,0)) != 1)
                    seq.PushBack(data[i]);
            #endregion
            /*-------------------------------------------------------------------------------------*/

        }


    }
}
