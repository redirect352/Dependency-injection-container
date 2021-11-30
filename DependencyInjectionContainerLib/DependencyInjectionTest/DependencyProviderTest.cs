using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DependencyInjectionContainerLib.Config;
using DependencyInjectionContainerLib;
using System.Collections.Generic;
using Moq;


namespace DependencyInjectionTest
{
    [TestClass]
    public class DependencyProviderTest
    {
        Mock<DependencyConfig> mock;
        [TestInitialize]
        public void Initialize()
        {
            mock = new Mock<DependencyConfig>();
        }


        [TestMethod]
        public void TestDependencyInjectionWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.InstancePerDependency));
            mock.Setup(a => a.GetImplementationInfo(typeof(IDepend2), -1))
                           .Returns(retValue);
            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res = provider.Resolve<IDepend2>();

            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(TestClass));
        }


        [TestMethod]
        public void TestRegistrationAsSelfWithoutConfig()
        {
            
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.InstancePerDependency));
            mock.Setup(a => a.GetImplementationInfo(typeof(TestClass), -1))
                           .Returns(retValue);

            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res = provider.Resolve<TestClass>();
            Assert.IsInstanceOfType(res, typeof(TestClass));
        }

        [TestMethod]
        public void TestOpenGenericRealizationWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.InstancePerDependency));
            mock.Setup(a => a.GetImplementationInfo(typeof(IDepend3), -1))
                           .Returns(retValue);
            var openGenericRetValue = new List<InterfaceImplementation>();
            openGenericRetValue.Add(new InterfaceImplementation(typeof(Realization<IDepend3>),ImplementationsTTL.InstancePerDependency));
            mock.Setup(a => a.GetImplementationInfo(typeof(IDepend<IDepend3>), -1))
                           .Returns(openGenericRetValue);
            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res = provider.Resolve<IDepend<IDepend3>>();
            Assert.IsInstanceOfType(res, typeof(Realization<IDepend3>));


        }

        [TestMethod]
        public void TestSingletonWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.Singleton));
            mock.Setup(a => a.GetImplementationInfo(typeof(IDepend3), -1))
                           .Returns(retValue);
            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res1 = provider.Resolve<IDepend3>();
            var res2 = provider.Resolve<IDepend3>();
            Assert.IsInstanceOfType(res1, typeof(TestClass));
            Assert.AreSame(res1, res2);
        }

        [TestMethod]


        public void TestInstancePerDependencyWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.InstancePerDependency));
            mock.Setup(a => a.GetImplementationInfo(typeof(IDepend3), -1))
                           .Returns(retValue);
            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res1 = provider.Resolve<IDepend3>();
            var res2 = provider.Resolve<IDepend3>();
            Assert.IsInstanceOfType(res1, typeof(TestClass));
            Assert.AreNotSame(res1, res2);
        }


        [TestMethod]
        public void TestTwoImplementationsWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.Singleton));
            retValue.Add(new InterfaceImplementation(typeof(TestClass2), ImplementationsTTL.Singleton));

            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] {typeof(IDepend2),typeof(IEnumerable<IDepend2>) }), -1))
                           .Returns(retValue);
            DependencyProvider provider = new DependencyProvider(mock.Object);
            var res = provider.Resolve<IEnumerable<IDepend2>>();
            int count = 0;
            foreach (IDepend2 depend2 in res)
            {
                count++;
            }
            Assert.AreEqual(count, 2);
        }

        [TestMethod]
        public void TestTwoImplementationsWithDifferenceWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.Singleton,0));
            retValue.Add(new InterfaceImplementation(typeof(TestClass2), ImplementationsTTL.Singleton,1));

            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] { typeof(IDepend2) }), -1))
                           .Returns(retValue);
            var retValue1 = new List<InterfaceImplementation>();
            retValue1.Add(retValue[0]);
            var retValue2 = new List<InterfaceImplementation>();
            retValue2.Add(retValue[1]);

            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] { typeof(IDepend2) }), 0))
                           .Returns(retValue1);
            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] { typeof(IDepend2) }), 1))
                           .Returns(retValue2);



            DependencyProvider provider = new DependencyProvider(mock.Object);

            int? test2Numb = provider.GetRealizationNumber(typeof(IDepend2), typeof(TestClass2)),
                test1Numb = provider.GetRealizationNumber(typeof(IDepend2), typeof(TestClass));
            Assert.IsNotNull(test2Numb);
            Assert.IsNotNull(test1Numb);
            var res = provider.Resolve<IDepend2>(test2Numb.Value);
            var res2 = provider.Resolve<IDepend2>(test1Numb.Value);
            Assert.IsInstanceOfType(res, typeof(TestClass2));
            Assert.IsInstanceOfType(res2, typeof(TestClass));
        }


        [TestMethod]
        public void TestNestedDependencyWithoutConfig()
        {
            var retValue = new List<InterfaceImplementation>();
            retValue.Add(new InterfaceImplementation(typeof(TestNestedDependencyClass), ImplementationsTTL.InstancePerDependency));

            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] { typeof(IDepend2) }), -1))
                           .Returns(retValue);

            var retValue1 = new List<InterfaceImplementation>();
            retValue1.Add(new InterfaceImplementation(typeof(TestClass), ImplementationsTTL.InstancePerDependency));

            mock.Setup(a => a.GetImplementationInfo(It.IsIn<Type>(new Type[] {  typeof(IDepend3) }), -1))
                           .Returns(retValue1);


            //dependencyConfig.Register<IDepend2, TestNestedDependencyClass>(ImplementationsTTL.Singleton);
            //dependencyConfig.Register<IDepend3, TestClass>(ImplementationsTTL.Singleton);

            DependencyProvider provider = new DependencyProvider(mock.Object);

            var res = provider.Resolve<IDepend2>();
            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(TestNestedDependencyClass));
            Assert.IsNotNull(((TestNestedDependencyClass)res).Depend3);
            Assert.IsInstanceOfType(((TestNestedDependencyClass)res).Depend3, typeof(TestClass));
        }



    




    }
}
