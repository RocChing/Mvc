﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.AspNet.Mvc.DefaultControllerTypeProviderControllers;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class DefaultControllerTypeProviderTest
    {
        [Fact]
        public void IsController_UserDefinedClass()
        {
            // Arrange
            var typeInfo = typeof(StoreController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        [Fact]
        public void IsController_FrameworkControllerClass()
        {
            // Arrange
            var typeInfo = typeof(Controller).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_UserDefinedControllerClass()
        {
            // Arrange
            var typeInfo = typeof(DefaultControllerTypeProviderControllers.Controller).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_Interface()
        {
            // Arrange
            var typeInfo = typeof(IController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_AbstractClass()
        {
            // Arrange
            var typeInfo = typeof(AbstractController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_DerivedAbstractClass()
        {
            // Arrange
            var typeInfo = typeof(DerivedAbstractController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        [Fact]
        public void IsController_OpenGenericClass()
        {
            // Arrange
            var typeInfo = typeof(OpenGenericController<>).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_WithoutSuffixOrAncestorWithController()
        {
            // Arrange
            var typeInfo = typeof(NoSuffixPoco).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.False(isController);
        }

        [Fact]
        public void IsController_ClosedGenericClass()
        {
            // Arrange
            var typeInfo = typeof(OpenGenericController<string>).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        [Fact]
        public void IsController_DerivedGenericClass()
        {
            // Arrange
            var typeInfo = typeof(DerivedGenericController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        [Fact]
        public void IsController_Poco_WithNamingConvention()
        {
            // Arrange
            var typeInfo = typeof(PocoController).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        [Fact]
        public void IsController_NoControllerSuffix()
        {
            // Arrange
            var typeInfo = typeof(NoSuffix).GetTypeInfo();
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(typeInfo);

            // Assert
            Assert.True(isController);
        }

        public static TheoryData TypesWithPocoControllerAncestor
        {
            get
            {
                return new TheoryData<TypeInfo>
                {
                   typeof(ChildWithoutSuffix).GetTypeInfo(),
                   typeof(DescendantLevel1).GetTypeInfo(),
                   typeof(DescendantLevel2).GetTypeInfo()
                };
            }
        }

        [Theory]
        [InlineData(typeof(ChildWithoutSuffix))]
        [InlineData(typeof(DescendantLevel1))]
        [InlineData(typeof(DescendantLevel2))]
        public void IsController_ReturnsTrue_IfAncestorTypeNameHasControllerSuffix(Type type)
        {
            // Arrange
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(type.GetTypeInfo());

            // Assert
            Assert.True(isController);
        }

        [Theory]
        [InlineData(typeof(BaseNonControllerController))]
        [InlineData(typeof(BaseNonControllerControllerChild))]
        [InlineData(typeof(BasePocoNonControllerController))]
        [InlineData(typeof(BasePocoNonControllerControllerChild))]
        [InlineData(typeof(NonController))]
        [InlineData(typeof(NonControllerChild))]
        [InlineData(typeof(PersonModel))]
        public void IsController_ReturnsFalse_IfTypeOrAncestorHasNonControllerAttribute(Type type)
        {
            // Arrange
            var provider = GetControllerTypeProvider();

            // Act
            var isController = provider.IsController(type.GetTypeInfo());

            // Assert
            Assert.False(isController);
        }

        private static DefaultControllerTypeProvider GetControllerTypeProvider()
        {
            var assemblyProvider = new FixedSetAssemblyProvider();
            return new DefaultControllerTypeProvider(assemblyProvider, NullLoggerFactory.Instance);
        }
    }
}

// These controllers are used to test the DefaultControllerTypeProvider implementation
// which REQUIRES that they be public top-level classes. To avoid having to stub out the
// implementation of this class to test it, they are just top level classes. Don't reuse
// these outside this test - find a better way or use nested classes to keep the tests
// independent.
namespace Microsoft.AspNet.Mvc.DefaultControllerTypeProviderControllers
{
    public abstract class AbstractController : Mvc.Controller
    {
    }

    public class DerivedAbstractController : AbstractController
    {
    }

    public class StoreController : Mvc.Controller
    {
    }

    public class Controller
    {
    }

    public class OpenGenericController<T> : Mvc.Controller
    {
    }

    public class DerivedGenericController : OpenGenericController<string>
    {
    }

    public interface IController
    {
    }

    public class NoSuffix : Mvc.Controller
    {
    }

    public class NoSuffixPoco
    {

    }

    public class PocoController
    {
    }

    public class CustomBaseController
    {

    }

    public abstract class CustomAbstractBaseController
    {

    }

    public class ChildWithoutSuffix : CustomBaseController
    {

    }

    public class DescendantLevel1 : CustomBaseController
    {

    }

    public class DescendantLevel2 : DescendantLevel1
    {

    }

    public class AbstractChildWithoutSuffix : CustomAbstractBaseController
    {

    }

    [NonController]
    public class BasePocoNonControllerController
    {

    }

    public class BasePocoNonControllerControllerChild : BasePocoNonControllerController
    {

    }

    [NonController]
    public class BaseNonControllerController : Controller
    {

    }

    public class BaseNonControllerControllerChild : BaseNonControllerController
    {

    }

    [NonController]
    public class PocoNonController
    {

    }

    [NonController]
    public class NonControllerChild : Controller
    {

    }

    [NonController]
    public class NonController : Controller
    {

    }

    public class DataModelBase
    {

    }

    public class EntityDataModel : DataModelBase
    {

    }

    public class PersonModel : EntityDataModel
    {

    }
}