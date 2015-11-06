using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper
{

    /// <summary>
    /// Extensions for properties management
    /// </summary>
    public static class PropertyExtensions
    {
        const string WrongExpressionMessage = "Expression invalid. Must be called with an expression of type n() => PropertyName!";
        const string WrongUnaryExpressionMessage = "Unary-expression invalid. Must be called with an expression of type n() => PropertyName!";

        /// <summary>
        /// Helper to find a member expression
        /// </summary>
        private static MemberExpression FindMemberExpression<T>(Expression<Func<T>> expression)
        {
            if (expression.Body is UnaryExpression)
            {
                var unary = (UnaryExpression)expression.Body;
                var member = unary.Operand as MemberExpression;
                if (member == null)
                    throw new ArgumentException(WrongUnaryExpressionMessage, "expression");
                return member;
            }
            return expression.Body as MemberExpression;
        }

        /// <summary>
        /// Get the property name from an expression
        /// </summary>
        public static String GetPropertyName<T>(this object target, Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            // Find the expression
            var memberExpression = FindMemberExpression(expression);
            if (memberExpression == null)
            {
                throw new ArgumentException(WrongExpressionMessage, "expression");
            }

            // Find the member
            var member = memberExpression.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException(WrongExpressionMessage, "expression");
            }

            // Reject the member from another class
            if (target != null && member.DeclaringType != null)
            {
                if (!member.DeclaringType.GetTypeInfo().IsAssignableFrom(target.GetType().GetTypeInfo()))
                {
                    throw new ArgumentException(WrongExpressionMessage, "expression");
                }
            }

            // Reject static member
            if (member.GetMethod.IsStatic)
            {
                throw new ArgumentException(WrongExpressionMessage, "expression");
            }

            // Returns the name
            return member.Name;
        }

    }
}
