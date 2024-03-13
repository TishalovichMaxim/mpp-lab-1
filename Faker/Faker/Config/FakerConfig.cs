using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FakerGenerator;

namespace DtoGenerator.Config;

public class FakerConfig
{
    private Dictionary<Type, Dictionary<(string, Type), IGenerator>> _configGenerators = new();

    public void Add<T, V, G>(Expression<Func<T, V>> expression)
    {
        ExpressionType expressionType = expression.Body.NodeType;
        if (expressionType != ExpressionType.MemberAccess)
        {
            throw new FakerConfigException();
        }

        MemberExpression memberExpression = (MemberExpression)expression.Body;
        string memberName = memberExpression.Member.Name;
        string constructorParameterName = memberName[..1].ToLower() + memberName[1..];
        
        if (!_configGenerators.ContainsKey(typeof(T)))
        {
            _configGenerators[typeof(T)] = new Dictionary<(string, Type), IGenerator>();
        }

        _configGenerators[typeof(T)][(constructorParameterName, typeof(V))] = (IGenerator)Activator.CreateInstance(typeof(G))!;
    }
}
