namespace Kantaiko.Hosting.Modularity.TypeRegistration;

public interface ITypeRegistrationHandler
{
    bool Handle(Type type);

    void Complete();
}
