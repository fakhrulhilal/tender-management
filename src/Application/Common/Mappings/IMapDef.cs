namespace TenderManagement.Application.Common.Mappings
{
    /// <summary>
    /// Define mapping from a class to type <typeparamref name="T"/> and vice versa
    /// </summary>
    /// <typeparam name="T">Target conversion type</typeparam>
    public interface IMapDef<T>
    {
        void Mapping(AutoMapper.Profile profile) => profile.CreateMap(GetType(), typeof(T));
    }
}
