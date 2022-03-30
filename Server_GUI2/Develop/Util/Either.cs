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

        public abstract Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, Either<T, FAILURE>> func);

        public abstract Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func);

        public abstract Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, Either<SUCCESS, T>> func);

        public abstract Either<EitherVoid, FAILURE> SuccessAction(Action<SUCCESS> action);

        public abstract Either<SUCCESS, EitherVoid> FailureAction(Action<FAILURE> action);

        public abstract SUCCESS SuccessOrDefault(SUCCESS defaultValue);

        public abstract SUCCESS SuccessOrFunc(Func<FAILURE, SUCCESS> failureToSuccess);

        public abstract FAILURE FailureOrDefault(FAILURE defaultValue);

        public abstract FAILURE FailureOrFunc(Func<SUCCESS, FAILURE> successToFailure);
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

        public override Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, Either<T, FAILURE>> func) => func(Value);

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func) => new Success<SUCCESS, T>(Value);

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, Either<SUCCESS, T>> func) => new Success<SUCCESS, T>(Value);

        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => Value;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => defaultValue;

        public override Either<EitherVoid, FAILURE> SuccessAction(Action<SUCCESS> action) {
            action(Value);
            return new Success<EitherVoid, FAILURE>(EitherVoid.Instance);
        }

        public override Either<SUCCESS, EitherVoid> FailureAction(Action<FAILURE> action) => new Success<SUCCESS, EitherVoid>(Value);

        public override SUCCESS SuccessOrFunc(Func<FAILURE, SUCCESS> failureToSuccess) => Value;

        public override FAILURE FailureOrFunc(Func<SUCCESS, FAILURE> successToFailure) => successToFailure(Value);
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

        public override Either<T, FAILURE> SuccessFunc<T>(Func<SUCCESS, Either<T, FAILURE>> func) => new Failure<T, FAILURE>(Value);

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, T> func) => new Failure<SUCCESS, T>(func(Value));

        public override Either<SUCCESS, T> FailureFunc<T>(Func<FAILURE, Either<SUCCESS, T>> func) => func(Value);
        
        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => defaultValue;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => Value;

        public override Either<EitherVoid, FAILURE> SuccessAction(Action<SUCCESS> action) => new Failure<EitherVoid, FAILURE>(Value);

        public override SUCCESS SuccessOrFunc(Func<FAILURE, SUCCESS> failureToSuccess) => failureToSuccess(Value);

        public override FAILURE FailureOrFunc(Func<SUCCESS, FAILURE> successToFailure) => Value; 
        
        public override Either<SUCCESS, EitherVoid> FailureAction(Action<FAILURE> action)
        {
            action(Value);
            return new Failure<SUCCESS, EitherVoid>(EitherVoid.Instance);
        }
    }

    public class EitherVoid
    {
        public static EitherVoid Instance { get; } = new EitherVoid();
        private EitherVoid() { }
    }
}
