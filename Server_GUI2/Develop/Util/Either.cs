using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2.Develop.Util
{
    abstract class Either<SUCCESS,FAILURE>
    {
        public abstract bool IsSuccess { get; }

        public abstract Either<T, FAILURE> SuccessMap<T>(Func<SUCCESS, T> func);

        public abstract Either<SUCCESS, T> FailureMap<T>(Func<FAILURE, T> func);

        public abstract SUCCESS SuccessOrDefault(SUCCESS defaultValue);

        public abstract FAILURE FailureOrDefault(FAILURE defaultValue);
    }

    class Success<SUCCESS, FAILURE>: Either<SUCCESS, FAILURE>
    {
        public override bool IsSuccess { get; } = true;

        private SUCCESS Value { get; }

        public Success(SUCCESS value)
        {
            Value = value;
        }

        public override Either<T, FAILURE> SuccessMap<T>(Func<SUCCESS, T> func) => new Success<T, FAILURE>(func(Value));

        public override Either<SUCCESS, T> FailureMap<T>(Func<FAILURE, T> func) => new Success<SUCCESS, T>(Value);

        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => Value;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => defaultValue;
    }

    class Failure<SUCCESS, FAILURE> : Either<SUCCESS, FAILURE>
    {
        public override bool IsSuccess { get; } = false;

        private FAILURE Value { get; }

        public Failure(FAILURE value)
        {
            Value = value;
        }

        public override Either<T, FAILURE> SuccessMap<T>(Func<SUCCESS, T> func) => new Failure<T, FAILURE>(Value);

        public override Either<SUCCESS, T> FailureMap<T>(Func<FAILURE, T> func) => new Failure<SUCCESS, T>(func(Value));

        public override SUCCESS SuccessOrDefault(SUCCESS defaultValue) => defaultValue;

        public override FAILURE FailureOrDefault(FAILURE defaultValue) => Value;

    }
}
