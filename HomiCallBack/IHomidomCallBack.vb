Imports System
Imports System.ServiceModel
Imports System.Timers

Namespace HoMIDom

    Public Class IHomidomCallBack
        <ServiceContract(CallbackContract:=GetType(IMyServiceCallback))> _
        Public Interface IMyService2
            <OperationContract()> _
            Sub OpenSession()
        End Interface

        Public Interface IMyServiceCallback
            <OperationContract()> _
            Sub OnCallback()
        End Interface

    End Class

    '<ServiceBehavior(ConcurrencyMode:=ConcurrencyMode.Reentrant)>
    <ServiceBehavior(ConcurrencyMode:=ConcurrencyMode.Reentrant)> Public Class MyServiceCallBack
        Implements IHomidomCallBack.IMyService2

        Public Shared Callback As IHomidomCallBack.IMyServiceCallback
        Public Shared Timer As Timer

        Public Sub OpenSession() Implements IHomidomCallBack.IMyService2.OpenSession
            Callback = OperationContext.Current.GetCallbackChannel(Of IHomidomCallBack.IMyServiceCallback)()

            Timer = New Timer(1000)
            AddHandler Timer.Elapsed, AddressOf OnTimerElapsed
            Timer.Enabled = True
        End Sub

        Private Sub OnTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
            Callback.OnCallback()
        End Sub
    End Class
End Namespace