using Synaptrix;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.Interfaces
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : notnull
    {
    }
}
