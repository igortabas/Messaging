using System;
using System.Collections.Generic;
using System.Reflection;
using Brainstorm.Messaging.Abstractions;
using Brainstorm.Messaging.Infrastructure;
using Moq;
using Xunit;

namespace Brainstorm.MessagingTests
{
    public class TypeProviderTests
    {
        public static IEnumerable<object[]> GetMessageType_ReturnType_DataSource => new[]
        {
            new object[] { typeof(FakeAssembly).FullName, typeof(FakeAssembly) },
            new object[] { "test", null }
        };

        [Fact]
        public void GetMessageTypes_MessagingAssamblies_GetAllTypesFromThisAssamblies()
        {
            // arrange
            var fakeAssamlies = new List<Assembly>
            {
                new FakeAssembly("Brainstrom.Messaging", new[] { typeof(TypeProviderTests) }),
                new FakeAssembly("Brainstrom.GDPR.Messaging", new[] { typeof(FakeAssembly) }),
                new FakeAssembly("Brainstrom.GDPR", new[] { typeof(string) })
            };
            var proxyMock = new Mock<IAssemblyProxy>();
            proxyMock.Setup(x => x.GetAssemblies()).Returns(fakeAssamlies);
            var typeProvider = new TypeProvider(proxyMock.Object);

            // act
            var actualTypes = typeProvider.GetMessageTypes();

            // assert
            Assert.DoesNotContain(typeof(string), actualTypes);
            proxyMock.Verify(x => x.GetAssemblies(), Times.Once);
        }

        [Fact]
        public void GetMessageTypes_No_MessagingAssamblies_Empty()
        {
            // arrange
            var fakeAssamlies = new List<Assembly>
            {
                new FakeAssembly("Brainstrom.test", new[] { typeof(TypeProviderTests) }),
                new FakeAssembly("Brainstrom.GDPR2", new[] { typeof(FakeAssembly) }),
                new FakeAssembly("Brainstrom.GDPR", new[] { typeof(string) })
            };
            var proxyMock = new Mock<IAssemblyProxy>();
            proxyMock.Setup(x => x.GetAssemblies()).Returns(fakeAssamlies);
            var typeProvider = new TypeProvider(proxyMock.Object);

            // act
            var actualTypes = typeProvider.GetMessageTypes();

            // assert
            Assert.Empty(actualTypes);
            proxyMock.Verify(x => x.GetAssemblies(), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetMessageType_ReturnType_DataSource))]
        public void GetMessageType_ReturnType(string typeName, Type expected)
        {
            // arrange
            var fakeAssamlies = new List<Assembly>
            {
                new FakeAssembly("Brainstrom.Messaging", new[] { typeof(TypeProviderTests) }),
                new FakeAssembly("Brainstrom.GDPR.Messaging", new[] { typeof(FakeAssembly) }),
                new FakeAssembly("Brainstrom.GDPR", new[] { typeof(string) })
            };
            var proxyMock = new Mock<IAssemblyProxy>();
            proxyMock.Setup(x => x.GetAssemblies()).Returns(fakeAssamlies);
            var typeProvider = new TypeProvider(proxyMock.Object);

            // act
            var actualType = typeProvider.GetMessageType(typeName);

            // assert
            Assert.Equal(expected, actualType);
        }

        private class FakeAssembly : Assembly
        {
            private readonly Type[] types;

            public FakeAssembly(string fullName, Type[] types)
            {
                this.types = types;
                this.FullName = fullName;
            }

            public override string FullName { get; }

            public override Type[] GetTypes()
            {
                return this.types;
            }
        }
    }
}
