using System;

public interface IHasActionValidation<T> 
{
    event Func<T, bool> OnResquestValidation;
}
