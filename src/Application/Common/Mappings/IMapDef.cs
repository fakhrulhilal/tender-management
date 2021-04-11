namespace TenderManagement.Application.Common.Mappings
{
    /// <summary>
    /// Define mapping from a class to type <typeparamref name="T"/> and vice versa
    /// </summary>
    /// <typeparam name="T">Target conversion type</typeparam>
    public interface IMapDef<T>
    {
        /// <summary>
        /// Define default mapping from current entity to target and vice versa
        /// </summary>
        /// <param name="profile"></param>
        void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap(GetType(), typeof(T));
            profile.CreateMap(typeof(T), GetType());
        }
    }
}
