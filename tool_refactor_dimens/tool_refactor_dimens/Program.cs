

using System.Text.RegularExpressions;
using System.Xml;

// Declare
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

string[] listAllPathFileDelete = new string[]
{
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Utilities\MobileConstants.cs",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.Core\iOrder.Utilities\Constants\GlobalConstant.cs",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_1920x1080.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_1920x1080_240.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_1920x1440.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_720x1280.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_720x1440.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_Large.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_Normal.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_Small.xaml",
    @"C:\Users\GF\Documents\UGOTechs\thanh\iOrder\iOrder.AppFlows\iOrder.Mobile\Resources\Styles\Dimens_Small_Landscape.xaml",
};


///Main

findListKeyUnused();
//deleteAllfile();


Console.WriteLine("DONE");
Console.ReadLine();

/// FUNCTION

//Auto delete key in all file 
void deleteAllfile()
{
    var lineKeys = File.ReadAllLines(list_key_delete);
    foreach(var lineKey in lineKeys)
    {
        foreach(var file in listAllPathFileDelete)
        {
            if (file.Contains(".xaml"))
                deleteKeyUnused(file, lineKey);
            else
                deleteKeyUnusedFileCs(file, lineKey);
        }
    }
}

// func delete file .xaml
void deleteKeyUnused(string filePath, string key)
{
    key = "\"" + key + "\"";
    var lines = File.ReadAllLines(filePath).ToList();
    int indexToRemove = lines.FindIndex(line => line.Contains(key));

    if (indexToRemove < 0)
        return;
    if (lines[indexToRemove].Contains("/>") || lines[indexToRemove].Contains("</"))
    {
        lines.RemoveAt(indexToRemove);
        Console.WriteLine($"{lines[indexToRemove]}");
    }
    else if (!lines[indexToRemove].Contains(">"))
    {
        for (int i = indexToRemove; i < lines.Count; i++)
        {
            Console.WriteLine(lines[i]);
            if (lines[i].Contains("/>"))
            {
                lines.RemoveRange(indexToRemove, i - indexToRemove + 1);
                break;
            }
        }
    }
    else
    {
        int startIndex = lines[indexToRemove].IndexOf('<');
        int endIndex = lines[indexToRemove].IndexOf(' ', startIndex);
        if (endIndex == -1)
        {
            endIndex = lines[indexToRemove].IndexOf("x:Key", startIndex);
        }

        string result = lines[indexToRemove].Substring(startIndex + 1, endIndex - startIndex - 1);

        string closingTag = $"</{result}>";

        for (int i = indexToRemove; i< lines.Count; i++)
        {
            Console.WriteLine(lines[i]);
            if (lines[i].Contains(closingTag)) 
            {
                lines.RemoveRange(indexToRemove, i - indexToRemove + 1);
                break;
            }
        }
    }
    File.WriteAllLines(filePath, lines);
}

//func delete file .cs
void deleteKeyUnusedFileCs(string filePath, string key)
{
    key = "\"" + key + "\"";
    List<string> lines = File.ReadAllLines(filePath).ToList();
    int indexToRemove = lines.FindIndex(line => line.Contains(key));
    if (indexToRemove < 0)
        return;
    Console.WriteLine(lines[indexToRemove]);
    lines.RemoveAt(indexToRemove);
    File.WriteAllLines(filePath, lines);
}

void findListKeyUnused()
{
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
}

static bool CheckCountFilter(string filePath, string filter)
{
    filter = filter.Replace(" ", "");
    string content = File.ReadAllText(filePath);
    string newfilter = "StaticResource " + filter;

    if (filePath.EndsWith("GlobalConstant.cs"))
    {
        filter = "\"" + filter + "\"";
        return FilterAppearsTwice(content, filter);
    }

    return content.Contains(newfilter); 
}

static bool FilterAppearsTwice(string content, string filter)
{
    int count = 0;
    int index = 0;

    while ((index = content.IndexOf(filter, index)) != -1)
    {
        count++;
        if (count >= 2) return true;
        index += filter.Length;
    }

    return false;
}

static bool FileContainsString(string filePath, string filter)
{
    try
    {
        if (File.Exists(filePath))
        {
            filter = filter.Replace(" ", "");
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