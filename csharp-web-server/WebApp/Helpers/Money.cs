


namespace WebApp.Data;

public static class Money
{
    public static string ConvertToWords(double number)
    {
        //split the number into integer part and two decimals
        string[] n = string.Join("", number.ToString("N2").Where(x => Char.IsNumber(x) || x is '.')).Split('.');
        return string.Format("{0} Saudi Riyals and {1} Hallalas", NumberToWords(int.Parse(n[0])), NumberToWords(int.Parse(n[1]))); 
    }

    private static string NumberToWords(int number)
    {
        if (number == 0)
            return "zero";

        if (number < 0)
            return "minus " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 1000000) > 0)
        {
            words += NumberToWords(number / 1000000) + " million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            Span<string> unitsMap = [ "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" ];
            Span<string> tensMap  = [ "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" ];

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words;
    }
}