using System;
using System.Linq.Expressions;

namespace Samba.Persistance.Specification
{
    public sealed class OrSpecification<T>
         : CompositeSpecification<T>
         where T : class
    {
        private readonly ISpecification<T> _rightSideSpecification;
        private readonly ISpecification<T> _leftSideSpecification;

        public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            _leftSideSpecification = leftSide ?? throw new ArgumentNullException("leftSide");
            _rightSideSpecification = rightSide ?? throw new ArgumentNullException("rightSide");
        }

        public override ISpecification<T> LeftSideSpecification
        {
            get { return _leftSideSpecification; }
        }

        public override ISpecification<T> RightSideSpecification
        {
            get { return _rightSideSpecification; }
        }

        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            Expression<Func<T, bool>> left = _leftSideSpecification.SatisfiedBy();
            Expression<Func<T, bool>> right = _rightSideSpecification.SatisfiedBy();

            return (left.Or(right));
        }
    }
}
