namespace KlabTestFramework.Core;

public class Demo
{
    private readonly IDemoDownloader _demoDownloader;

    public string Name { get; }

    public Demo(string name, IDemoDownloader demoDownloader)
    {
        Name = name;
        _demoDownloader = demoDownloader;
    }

    public async Task<string> GetDownloadedNameAsync()
    {
        return await _demoDownloader.DownloadNameAsync();
    }
}

public interface IDemoDownloader
{
    Task<string> DownloadNameAsync();
}
