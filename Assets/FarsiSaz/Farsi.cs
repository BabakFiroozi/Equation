

namespace Equation.FarsiSaz
{
    public static class Farsi
    {
        public static string Fix(string str, bool useNBidi = false)
        {
            string fixedText = "";
            if (useNBidi)
                fixedText = NBidi.NBidi.LogicalToVisual(str);
            else
                fixedText = Fa.faConvert(str);
            return fixedText;
        }
    }
}