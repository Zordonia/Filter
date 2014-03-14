using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.Filters
{
    public static class ObjectFilter<TObject>
    {
        public static Expression<Func<TObject, bool>> Filter { get; set; }
        public static Func<TObject, bool> Finder<TProperty>(
            Expression<Func<TObject, TProperty>> selector,
            TProperty value)
        {
            var param = Expression.Parameter(typeof(TObject), "x");
            var body = Expression.Equal(Expression.Invoke(selector, param), Expression.Constant(value, typeof(TProperty)));
            Expression<Func<TObject, bool>> predicate = Expression.Lambda<Func<TObject, bool>>(body, param);
            
            return predicate.Compile();
        }

        public static IEnumerable<Expression<Func<TObject,bool>>> Find<TProperty>(
            Expression<Func<TObject, TProperty>> selector,
            IEnumerable<TProperty> values)
        {
            IList<Expression<Func<TObject, bool>>> predicates = new List<Expression<Func<TObject, bool>>>();
            foreach (TProperty property in values)
            {
                var param = Expression.Parameter(typeof(TObject), "x");
                var body = Expression.Equal(Expression.Invoke(selector, param), Expression.Constant(property, typeof(TProperty)));
                Expression<Func<TObject, bool>> predicate = Expression.Lambda<Func<TObject, bool>>(body, param);
                predicates.Add(predicate);
            }
            return predicates;
        }

        public static Func<TObject, bool> FindAll<TProperty>(
            Expression<Func<TObject, TProperty>> selector,
            IEnumerable<TProperty> values)
        {
            var predicates = Find(selector, values);
            return predicates.And().Compile();
        }

        public static Func<TObject, bool> FindSome<TProperty>(
            Expression<Func<TObject, TProperty>> selector,
            IEnumerable<TProperty> values)
        {
            var predicates = Find(selector, values);
            return predicates.Or().Compile();
        }
    }

    public static class ObjectFilterEx
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> @this,
            params Expression<Func<T, bool>>[] predicates)
        {
            List<Expression<Func<T, bool>>> expressions = new List<Expression<Func<T, bool>>>()
            {
                @this
            };
            expressions.AddRange(predicates);
            return And(expressions);
        }

        public static Expression<Func<T, bool>> And<T>(
            this IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            Expression body = null;
            ParameterExpression p = null;
            Expression<Func<T, bool>> first = null;

            foreach (Expression<Func<T, bool>> item in predicates)
            {
                if (first == null)
                {
                    first = item;
                }
                else
                {
                    if (body == null)
                    {
                        body = first.Body;
                        p = first.Parameters[0];
                    }

                    var toReplace = item.Parameters[0];
                    var itemBody = ReplacementVisitor.Transform(item, toReplace, p);
                    body = Expression.AndAlso(body, itemBody);
                }
            }
            if (first == null)
            {
                throw new ArgumentException("No elements in sequence.");
            }
            return body == null ? first : Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> @this,
            params Expression<Func<T, bool>>[] predicates
            )
        {
            List<Expression<Func<T, bool>>> expressions = new List<Expression<Func<T, bool>>>(){
                @this
            };
            expressions.AddRange(predicates);
            return Or(expressions);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            Expression body = null;
            ParameterExpression p = null;
            Expression<Func<T, bool>> first = null;

            foreach (Expression<Func<T, bool>> item in predicates)
            {
                if (first == null)
                {
                    first = item;
                }
                else
                {
                    if (body == null)
                    {
                        body = first.Body;
                        p = first.Parameters[0];
                    }

                    var toReplace = item.Parameters[0];
                    var itemBody = ReplacementVisitor.Transform(item, toReplace, p);
                    body = Expression.OrElse(body, itemBody);
                }
            }
            if (first == null)
            {
                throw new ArgumentException("No elements in sequence.");
            }
            return body == null ? first : Expression.Lambda<Func<T, bool>>(body, p);
        }

        private sealed class ReplacementVisitor : ExpressionVisitor
        {
            private IList<ParameterExpression> SourceParameters { get; set; }
            private Expression ToFind { get; set; }
            private Expression ReplaceWith { get; set; }

            public static Expression Transform(
                LambdaExpression source,
                Expression toFind,
                Expression replaceWith)
            {
                var visitor = new ReplacementVisitor
                {
                    SourceParameters = source.Parameters,
                    ToFind = toFind,
                    ReplaceWith = replaceWith
                };
                return visitor.Visit(source.Body);
            }

            private Expression ReplaceNode(Expression node)
            {
                return node == ToFind ? ReplaceWith : node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                return ReplaceNode(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var result = ReplaceNode(node);
                if (result == node) result = base.VisitBinary(node);
                return result;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (SourceParameters.Contains(node)) return ReplaceNode(node);
                return SourceParameters.FirstOrDefault(p => p.Name == node.Name) ?? node;
            }
        }
    }
}
