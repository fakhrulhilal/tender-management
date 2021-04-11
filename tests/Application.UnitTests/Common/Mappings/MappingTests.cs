using AutoMapper;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using TenderManagement.Application.Common.Mappings;
using TenderManagement.Application.Tender.Command;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.Application.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }
        
        [Test]
        [TestCase(typeof(CreateTenderCommand), typeof(Domain.Entity.Tender))]
        [TestCase(typeof(UpdateTenderCommand), typeof(Domain.Entity.Tender))]
        [TestCase(typeof(Domain.Entity.Tender), typeof(GetTenderDetailQuery.Response))]
        [TestCase(typeof(Domain.Entity.Tender), typeof(GetTenderListQuery.Response))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = GetInstanceOf(source);

            _mapper.Map(instance, source, destination);
        }

        [Test]
        public void WhenMappingFromUpdateTenderCommandToTenderEntityThenItShouldKeepExistingAuditProperties()
        {
            string originalCreator = "iroel";
            var createdDate = DateTime.Now.Date;
            var entity = new Domain.Entity.Tender
            {
                CreatedBy = originalCreator,
                Created = createdDate
            };
            var command = new UpdateTenderCommand
            {
                Id = 1,
                Name = "coba"
            };

            _mapper.Map(command, entity);

            Assert.That(entity.CreatedBy, Is.EqualTo(originalCreator));
            Assert.That(entity.Created, Is.EqualTo(createdDate));
            Assert.That(entity.Id, Is.EqualTo(command.Id));
            Assert.That(entity.Name, Is.EqualTo(command.Name));
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);

            // Type without parameterless constructor
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}
