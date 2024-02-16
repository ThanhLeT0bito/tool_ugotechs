

using System.Text.RegularExpressions;
using System.Xml;

string folderPath = @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows";
string outputFilePath = @"C:\Users\GF\Documents\UGOTechs\tool\outputKey.txt";
//string filter = "NegativeBaseGroupSpacing";

string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
string[] filters = File.ReadAllLines(outputFilePath);

string[] excludedFiles = new string[]
        {
            @"MobileConstants.cs",
            @"Dimens_1920x1080.xaml",
            @"Dimens_1920x1080_240.xaml",
            @"Dimens_1920x1440.xaml",
            @"Dimens_720x1280.xaml",
            @"Dimens_720x1440.xaml",
            @"Dimens_Large.xaml",
            @"Dimens_Normal.xaml",
            @"Dimens_Small.xaml",
            @"Dimens_Small_Landscape.xaml"
        };


int totalFiles = 0;
bool check = false;
foreach (string filter in filters)
{
    totalFiles = 0;
    check = false;
    foreach (var file in files)
    {
        if (Array.IndexOf(excludedFiles, Path.GetFileName(file)) != -1 && !file.EndsWith(".dll"))
        {
            if (CountString(file, filter) > 1)
            {
                check = true;
                break;
            }
        }
        if (Array.IndexOf(excludedFiles, Path.GetFileName(file)) == -1 && FileContainsString(file, filter) && !file.EndsWith(".dll"))
        {
            totalFiles++;
        }
    }
    if (check)
        continue;
    if (totalFiles == 0)
        Console.WriteLine(filter);
}

//readAndWritexKey();

Console.WriteLine("DONE:");

Console.ReadLine();

static int CountString(string filePath, string filter)
{
    int count = 0;
    string line;

    using (StreamReader sr = new StreamReader(filePath))
    {
        while ((line = sr.ReadLine()) != null)
        {
            count += Regex.Matches(line, filter).Count;
        }
    }

    return count;
}

static bool FileContainsString(string filePath, string filter)
{
    try
    {
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            return content.Contains(filter);
        }
        else
        {
            return false;
        }
    }
    catch (Exception ex)
    {
        return false;
    }
}


static void readAndWritexKey()
{
    string filePath = @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_Small.xaml";
    string outputFilePath = @"C:\Users\GF\Documents\UGOTechs\tool\outputKey.txt";

    try
    {
        // Load tệp XML
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);

        // Duyệt qua các yếu tố
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            // Kiểm tra xem yếu tố có thuộc tính x:Key không
            XmlAttribute keyAttribute = node.Attributes?["x:Key"];
            if (keyAttribute != null)
            {
                // Ghi tên yếu tố vào tệp tin
                using (StreamWriter writer = new StreamWriter(outputFilePath, true))
                {
                    writer.WriteLine(keyAttribute.Value);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}