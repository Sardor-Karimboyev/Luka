namespace Luka.Types;

public class Language
{
    public Language(string oz, string uz, string ru, string en)
    {
        Oz = oz;
        Uz = uz;
        Ru = ru;
        En = en;
    }

    public string Oz { get; set; }
    public string Uz { get; set; }
    public string Ru { get; set; }
    public string En { get; set; }
}