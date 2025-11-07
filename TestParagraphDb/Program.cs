namespace TestParagraphDbService;

using ParagraphDb;

static class Program
{
    static void Main(string[] args)
    {
        Work wrk = new Work();
        wrk.Run();
    }
}

public class Work
{
    private IParagraph _parSer = new ParagraphDbService();
    
    public Work()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); 
        var dbPath = System.IO.Path.Combine(appDataPath, "FranDict","FranDict.db");
        _parSer.Setup(dbPath,false);
    }
    
    public void Run()
    {
        Console.WriteLine("__START__");

        var res = _parSer.FindParagraphs("дом");

        foreach (var r in res)
        {
            if(r != null)
                Console.WriteLine($"{r}");
        }
            
        
        Console.WriteLine("__END__");
    }
}