namespace FakerGenerator;

public class GeneratorAttribute : Attribute
{
    public Type GenereatorType
    { get; set; }

    public GeneratorAttribute(Type genereatorType)
    {
        GenereatorType = genereatorType;
    }
}
