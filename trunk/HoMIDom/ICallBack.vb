Imports System.ServiceModel

Namespace HoMIDom

    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface ICallBack
        <OperationContract()> Sub MessageToClient(ByVal Message As String)
    End Interface


End Namespace
