using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DependencyInjectionContainerLib.Config;
using DependencyInjectionContainerLib;
using System.Collections.Generic;

namespace DependencyInjectionTest
{
    [TestClass]
    public class DependencyTest
    {
        [TestMethod]
        public void TestOpenGenericRealization()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();        
            dependencyConfig.Register<IDepend2, TestClass>(ImplementationsTTL.InstancePerDependency);
            dependencyConfig.Register<IDepend3, TestClass>(ImplementationsTTL.InstancePerDependency);

            dependencyConfig.Register(typeof(IDepend<>), typeof(Realization<>), ImplementationsTTL.InstancePerDependency);

            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res = provider.Resolve<IDepend<IDepend3>>();

        }
        [TestMethod]
        public void TestRegistrationAsSelf()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();


            dependencyConfig.Register<TestClass, TestClass>(ImplementationsTTL.InstancePerDependency);
            
            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res = provider.Resolve<TestClass>();
            Assert.IsInstanceOfType(res, typeof(TestClass)); 


        }

        [TestMethod]
        public void TestSingleton()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();
            dependencyConfig.Register<IDepend3, TestClass>(ImplementationsTTL.Singleton);
            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res1 = provider.Resolve<IDepend3>();
            var res2 = provider.Resolve<IDepend3>();
            Assert.IsInstanceOfType(res1, typeof(TestClass));
            Assert.AreSame(res1,res2);
        }

        [TestMethod]
        public void TestInstancePerDependency()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();
            dependencyConfig.Register<IDepend3, TestClass>(ImplementationsTTL.InstancePerDependency);
            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res1 = provider.Resolve<IDepend3>();
            var res2 = provider.Resolve<IDepend3>();
            Assert.IsInstanceOfType(res1, typeof(TestClass));
            Assert.AreNotSame(res1, res2);
        }


        [TestMethod]
        public void TestTwoImplementations()
        {
            DependencyConfig dependencyConfig = new DependencyConfig();
            dependencyConfig.Register<IDepend2, TestClass>(ImplementationsTTL.Singleton);
            dependencyConfig.Register<IDepend2, TestClass2>(ImplementationsTTL.Singleton);

            DependencyProvider provider = new DependencyProvider(dependencyConfig);
            var res = provider.Resolve<IEnumerable<IDepend2>>();
            int count = 0;
            foreach (IDepend2 depend2 in res)
            {
                count++;
            }
            Assert.AreEqual(count,2);
        }





    }

    interface IDepend<TDep> where TDep:IDepend2
    {
    
    }
    interface IDepend2
    {

    }
    interface IDepend3 : IDepend2 { }

    class Realization<TDepend2> : IDepend<TDepend2> where TDepend2:IDepend2
    {
        private TDepend2 depend;
        public Realization(TDepend2 depend1)
        {
            depend = depend1;
        }

    }

    class TestClass :IDepend2, IDepend3
    {
        //private IDepend depend;
        //public TestClass(IDepend depend1) 
        //{
        //   depend = depend1;
        // }
    }
    class TestClass2 : IDepend2, IDepend3
    {
        //private IDepend depend;
        //public TestClass(IDepend depend1) 
        //{
        //   depend = depend1;
        // }
    }





}
