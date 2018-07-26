namespace AbifInterface
{
    public enum BaseType{A,T,C,G}
    interface IAbif
    {
        /// <summary>
        /// 按照DNA序列位点的各碱基电压峰值
        /// 第一个方括号内写位点数
        /// 第二个方括号内写碱基种类
        /// </summary>
        BaseData Seq { get; }
        BaseData Data { get; }
        void ReadFile(string sFileName);
    }
}