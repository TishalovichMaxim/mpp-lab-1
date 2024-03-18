using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DtoGenerator.Generator;

namespace DtoGenerator.Config;

public class FakerConfig
{
    public Dictionary<Type, Dictionary<(string, Type), IGenerator>> ConfigGenerators
    { get; } = new();

    public void Add<T, V, G>(Expression<Func<T, V>> expression)
    {
        ExpressionType expressionType = expression.Body.NodeType;
        if (expressionType != ExpressionType.MemberAccess)
        {
            throw new FakerConfigException();
        }

        MemberExpression memberExpression = (MemberExpression)expression.Body;
        string memberName = memberExpression.Member.Name;
        
        if (!ConfigGenerators.ContainsKey(typeof(T)))
        {
            ConfigGenerators[typeof(T)] = new Dictionary<(string, Type), IGenerator>();
        }

        ConfigGenerators[typeof(T)][(memberName, typeof(V))] = (IGenerator)Activator.CreateInstance(typeof(G))!;
    }
}
