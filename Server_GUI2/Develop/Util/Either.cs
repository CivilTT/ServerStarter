using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    public abstract class Either<SUCCESS,FAILURE>
    {
        public abstract bool IsSuccess { get; }

        public abstract Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, T> func);

        public abstract Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func);
        
        public abstract void SuccessAction(Action<SUCCESS> action);

        public abstract void FailureAction(Action<FAILURE> action);

        public abstract SUCCESS SuccessOrDefault(SUCCESS defaultValue);

        public abstract FAILURE FailureOrDefault(FAILURE defaultValue);
    }

    public class Success<SUCCESS, FAILURE>: Either<SUCCESS, FAILURE>
    {
        public override bool IsSuccess { get; } = true;

        private SUCCESS Value { get; }

        public Success(SUCCESS value)
        {
            Value = value;
        }

        public override Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, T> func) => new Success<T, FAILURE>(func(Value));

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func) => new Success<SUCCESS, T>(Value);

        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => Value;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => defaultValue;

        public override void SuccessAction(Action<SUCCESS> action) { action(Value); }

        public override void FailureAction(Action<FAILURE> action) { }
    }

    public class Failure<SUCCESS, FAILURE> : Either<SUCCESS, FAILURE>
    {
        public override bool IsSuccess { get; } = false;

        private FAILURE Value { get; }

        public Failure(FAILURE value)
        {
            Value = value;
        }

        public override Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, T> func) => new Failure<T, FAILURE>(Value);

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func) => new Failure<SUCCESS, T>(func(Value));

        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => defaultValue;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => Value;

        public override void SuccessAction(Action<SUCCESS> action) { }

        public override void FailureAction(Action<FAILURE> action) { action(Value); }

    }
}
