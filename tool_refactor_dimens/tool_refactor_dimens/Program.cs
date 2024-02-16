

using System.Text.RegularExpressions;
using System.Xml;

string folderPath = @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows";
string outputFilePath = @"C:\Users\GF\Documents\UGOTechs\tool\outputKey.txt";
string list_key_delete = @"C:\Users\GF\Documents\UGOTechs\tool\result_key_delete.txt";

var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                     .Where(file => file.EndsWith(".cs") || file.EndsWith(".xaml"))
                     .ToArray();
string[] filters = File.ReadAllLines(outputFilePath);

string[] excludedFiles = new string[]
        {
            @"MobileConstants.cs",
            @"GlobalConstant.cs",
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


///Main

int total = 0;
using (StreamWriter writer = new StreamWriter(list_key_delete, false))
{
    foreach (string filter in filters)
    {
        bool check = false;
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            if (Array.IndexOf(excludedFiles, fileName) != -1)
            {
                if (CheckCountFilter(file, filter))
                {
                    check = true;
                    break;
                }
            }
            else if (FileContainsString(file, filter))
            {
                check = true;
                break;
            }
        }
        if (!check)
        {
            writer.WriteLine(filter);
            Console.WriteLine(filter);
            total++;
        }
    }
}

//readAndWritexKey();

Console.WriteLine("DONE:" + total);

Console.ReadLine();

/// FUNCTION

static bool CheckCountFilter(string filePath, string filter)
{
    string content = File.ReadAllText(filePath);
    filter = "StaticResource " + filter;

    return content.Contains(filter); 
}

static bool FileContainsString(string filePath, string filter)
{
    try
    {
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            string filter1 = "StaticResource " + filter;
            string filter2 = "\"" + filter + "\"";
            string filter3 = "DimenConstants." + filter;
            return content.Contains(filter1) || content.Contains(filter2) || content.Contains(filter3);
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

//lọc key từ dimens small
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