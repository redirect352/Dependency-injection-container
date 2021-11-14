using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Config
{
    public class DependencyConfig :IDependencyConfig
    {
        private Dictionary<Type, List<InterfaceImplementation>> Dependencies { get; set; } = new Dictionary<Type, List<InterfaceImplementation>>();



        public void Register<TDependency, TImpl>(ImplementationsTTL time)
            where TDependency : class
            where TImpl : TDependency
        {
            Register(typeof(TDependency), typeof(TImpl), time);

        }

        public void Register(Type dependencyType, Type implementationType, ImplementationsTTL time)
        {
            if (!dependencyType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("Incompatible parameters type");
            }

            
            if (!Dependencies.ContainsKey(dependencyType))
            {
                Dependencies.Add(dependencyType, new List<InterfaceImplementation>());
            }
            Dependencies[dependencyType].Add(new InterfaceImplementation(implementationType, time));
        }

        internal List<InterfaceImplementation> GetImplementationInfo(Type type)
        {
            if (Dependencies.ContainsKey(type))
            {
                return Dependencies[type];
            }
            else
            {
                return new List<InterfaceImplementation>();
            }

        }



    }
}
