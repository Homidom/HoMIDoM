Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Linq
Imports System.Data

Namespace HoMIDom

    ''' <summary>
    ''' Liste toutes les functions et propriétés accessibles par les clients
    ''' </summary>
    ''' <remarks></remarks>
    <ServiceContract(Namespace:="http://HoMIDom/")> Public Interface IHoMIDom



#Region "Serveur"
        ''' <summary>
        ''' Retourne le paramètre de sauvegarde
        ''' </summary>
        ''' <param name="IdSrv">ID du serveur</param>
        ''' <returns>la valeur du paramètre, -1 si l'id du serveur est erronée</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetTimeSave(ByVal IdSrv As String) As Integer

        ''' <summary>
        ''' Fixe le paramètre de sauvegarde
        ''' </summary>
        ''' <param name="IdSrv">ID du serveur</param>
        ''' <param name="Value">Valeur positive qui définit toutes les X minutes une sauvegarde sera effectuée puis à l'arrêt du serveur</param>
        ''' <returns>0 si réussite</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SetTimeSave(ByVal IdSrv As String, ByVal Value As Integer) As String

        ''' <summary>
        ''' Retourne l'Id du serveur pour la communication SOAP
        ''' </summary>
        ''' <param name="IdSrv">ID du serveur</param>
        ''' <returns>ID du serveur ou 99 si l'IdSrv n'est pas celui du serveur (erreur)</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetIdServer(ByVal IdSrv As String) As String

        ''' <summary>
        ''' Fixe l'Id du serveur pour SOAP
        ''' </summary>
        ''' <param name="IdSrv">ID du serveur actuel</param>
        ''' <param name="Value">Nouvelle valeur de l'ID du serveur</param>
        ''' <returns>0 si l'ID a été pris en compte, 99 si l'IdSrv n'est pas celui du serveur (erreur),si l'ID est vide ça retourne "ERR: l'Id ne peut être null"</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SetIdServer(ByVal IdSrv As String, ByVal Value As String) As String

        ''' <summary>
        ''' Retourne la version du serveur
        ''' </summary>
        ''' <returns>Version du serveur</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetServerVersion() As String


        ''' <summary>
        ''' Retourne l'heure du serveur
        ''' </summary>
        ''' <returns>Heure du serveur</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetTime() As String

        ''' <summary>
        ''' Permet d'envoyer un message d'un client vers le server
        ''' </summary>
        ''' <param name="Message"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub MessageToServer(ByVal Message As String)

        ''' <summary>
        ''' Sauvegarde la configuration
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveConfig(ByVal IdSrv As String) As String


        ''' <summary>
        ''' Démarre le service et charge la config
        ''' </summary>
        ''' <remarks></remarks>
        <OperationContract()> Sub Start()


        ''' <summary>
        ''' Arrête le service
        ''' </summary>
        ''' <param name="idSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub [Stop](ByVal idSrv As String)

        ''' <summary>
        ''' Fixe la valeur de port SOAP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Valeur du port SOAP</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetPortSOAP(ByVal IdSrv As String, ByVal Value As Double)

        ''' <summary>
        ''' Retourne la valeur de port SOAP
        ''' </summary>
        ''' <returns>Valeur du port SOAP</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetPortSOAP() As Double

        ''' <summary>
        ''' Retourne la date et heure du dernier démarrage du serveur
        ''' </summary>
        ''' <returns>DateTime du dernier démarrage du serveur</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLastStartTime() As Date

        ''' <summary>
        ''' Valeur du levé du soleil
        ''' </summary>
        ''' <returns>Heure du levé du soleil</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureLeverSoleil() As String

        ''' <summary>
        ''' Valeur du couché du soleil
        ''' </summary>
        ''' <returns>Heure du couché du soleil</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCoucherSoleil() As String

        ''' <summary>
        ''' Obtenir la valeur de longitude
        ''' </summary>
        ''' <returns>Valeur de la longitude du serveur (pour calcul des heures de levé et couché du solseil)</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLongitude() As Double


        ''' <summary>
        ''' Appliquer une valeur de longitude
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Valeur de longitude</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetLongitude(ByVal IdSrv As String, ByVal Value As Double)

        ''' <summary>
        ''' Obtenir la valeur de latitude
        ''' </summary>
        ''' <returns>Valeur de la latitude du serveur (pour calcul des heures de levé et couché du solseil)</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetLatitude() As Double

        ''' <summary>
        ''' Appliquer la valeur de latitude
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Valeur de latitude</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetLatitude(ByVal IdSrv As String, ByVal Value As Double)

        ''' <summary>
        ''' Obtenir la correction sur l'heure du coucher du soleil
        ''' </summary>
        ''' <returns>Valeur de correction appliquée sur l'heure du coucher du soleil</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCorrectionCoucher() As Integer

        ''' <summary>
        ''' Appliquer une correction sur l'heure du coucher du soleil
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Valeur de correction appliquée sur l'heure du couché du soleil</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetHeureCorrectionCoucher(ByVal IdSrv As String, ByVal Value As Integer)

        ''' <summary>
        ''' Obtenir la correction sur l'heure de lever du soleil
        ''' </summary>
        ''' <returns>Valeur de correction appliquée sur l'heure du levé du soleil</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHeureCorrectionLever() As Integer

        ''' <summary>
        ''' Appliquer une correction sur l'heure du levé du soleil
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Valeur de correction appliquée sur l'heure du levé du soleil</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetHeureCorrectionLever(ByVal IdSrv As String, ByVal Value As Integer)

        ''' <summary>
        '''  Convert a file on a byte array.
        ''' </summary>
        ''' <param name="file">Chemin complet du fichier se trouvant sur le serveur</param>
        ''' <returns>Tableau de byte représentant le fichier</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetByteFromImage(ByVal file As String) As Byte()

        ''' <summary>
        ''' Retourne la liste de tous les fichiers image présents sur le serveur
        ''' </summary>
        ''' <returns>Liste (Type ImageFile) de tous les fichiers image (png, jpg) présents sur le serveur</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetListOfImage() As List(Of ImageFile)
#End Region

#Region "Historisation"
        ''' <summary>
        ''' Retourne la liste des sources histo (source et id) présents dans la base de données Homidom
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Liste (Type Historisation), Nothing si l'ID du serveur est erroné </returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllListHisto(ByVal IdSrv As String) As List(Of Historisation)

        ''' <summary>
        ''' Retourne l'historisation d'une source donnée
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Source">Source de la donnée</param>
        ''' <param name="idDevice">ID du device associé à la source de donnée</param>
        ''' <returns>List de type Historisation, 99 si l'ID du serveur est erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetHisto(ByVal IdSrv As String, ByVal Source As String, ByVal idDevice As String) As List(Of Historisation)
#End Region

#Region "Audio"
        ''' <summary>
        ''' Supprimer une extension Audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomExtension">Nom de l'exention à supprimer</param>
        ''' <returns>0 si exécuté, 99 si l'ID du serveur est erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String) As Integer

        ''' <summary>
        ''' Ajouter une nouvelle extension audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomExtension">Nom de la nouvelle extension</param>
        ''' <param name="Enable">True/False pour prendre en compte cette extension</param>
        ''' <returns>0 si exécuté, -1 si extension déjà existante, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function NewExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String, Optional ByVal Enable As Boolean = False) As Integer

        ''' <summary>
        ''' Active ou désactive une extension Audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomExtension">Nom de l'extension à activer/désactiver</param>
        ''' <param name="Enable">Activer ou Désactiver</param>
        ''' <returns>0 si exécuté, -1 si extension non trouvée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function EnableExtensionAudio(ByVal IdSrv As String, ByVal NomExtension As String, ByVal Enable As Boolean) As Integer

        ''' <summary>
        ''' Supprimer un répertoire Audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomRepertoire">Nom du répertoire à supprimer</param>
        ''' <returns>0 si exécuté, -1 si répertoire non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String) As Integer

        ''' <summary>
        ''' Ajouter un nouveau répertoire audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomRepertoire">Nom du nouveau répertoire</param>
        ''' <param name="Enable">Activer/Désactiver le répertoire</param>
        ''' <returns>0 si exécuté, -1 si répertoire déjà existant, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function NewRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String, Optional ByVal Enable As Boolean = False) As Integer

        ''' <summary>
        ''' Active ou désactive un répertoire Audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="NomRepertoire">Nom du répertoire</param>
        ''' <param name="Enable">Activer/désactiver</param>
        ''' <returns>0 si exécuté, -1 si répertoire non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function EnableRepertoireAudio(ByVal IdSrv As String, ByVal NomRepertoire As String, ByVal Enable As Boolean) As Integer

        ''' <summary>
        ''' Obtient la liste des répertoires audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type RepertoireAudio, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllRepertoiresAudio(ByVal IdSrv As String) As List(Of Audio.RepertoireAudio)

        ''' <summary>
        ''' Obtient la liste des extensions audio
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type ExtensionAudio, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllExtensionsAudio(ByVal IdSrv As String) As List(Of Audio.ExtensionAudio)
#End Region

#Region "User"
        ''' <summary>
        ''' Supprime un user
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="userId">ID du user à supprimer</param>
        ''' <returns>0 si exécuté, -1 si ID non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteUser(ByVal IdSrv As String, ByVal userId As String) As Integer

        ''' <summary>
        ''' Vérifie le couple username + login  renvoi true si ok
        ''' </summary>
        ''' <param name="Username">Username</param>
        ''' <param name="Password">Mot de passe</param>
        ''' <returns>True su identification OK sinon False</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function VerifLogin(ByVal Username As String, ByVal Password As String) As Boolean

        ''' <summary>
        ''' Permet de changer le mot de passe d'un user
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Username">Username</param>
        ''' <param name="OldPassword">Ancien mot de passe</param>
        ''' <param name="ConfirmNewPassword">Confirmation nouveau mot de passe</param>
        ''' <param name="Password">Nouveau mot de passe</param>
        ''' <returns>True si mot de passe modifié, False si erreur ou mauvais ID du serveur</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ChangePassword(ByVal IdSrv As String, ByVal Username As String, ByVal OldPassword As String, ByVal ConfirmNewPassword As String, ByVal Password As String) As Boolean

        ''' <summary>
        ''' Retourne un user par son username
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Username">Username</param>
        ''' <returns>Objet de type User correspondant au username</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnUserByUsername(ByVal IdSrv As String, ByVal Username As String) As Users.User

        ''' <summary>
        ''' Obtient la liste des users
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type User, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllUsers(ByVal IdSrv As String) As List(Of Users.User)

        ''' <summary>
        ''' Retourne un User par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="UserId">ID du User</param>
        ''' <returns>Objet de type User si trouvé, sinon Nothing si non trouvé ou ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnUserById(ByVal IdSrv As String, ByVal UserId As String) As Users.User

        ''' <summary>
        ''' Créer ou modifier un user
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="userId">ID du user (vide si c'est un nouveau, son ID pour le modifier)</param>
        ''' <param name="UserName">Username du user</param>
        ''' <param name="Password">Mot de passe du user</param>
        ''' <param name="Profil">Profil du user</param>
        ''' <param name="Nom">Nom du user</param>
        ''' <param name="Prenom">Prénom du user</param>
        ''' <param name="NumberIdentification">N° d'identification du user (peut être utilisé par exemple pour un N° de puce RFID)</param>
        ''' <param name="Image">Image du user</param>
        ''' <param name="eMail">eMail du user (utilisé lors d'envoi de mail dans une macro)</param>
        ''' <param name="eMailAutre">Second email du user</param>
        ''' <param name="TelFixe">Numéro de téléphone fixe du user</param>
        ''' <param name="TelMobile">Numéro de téléphone mobile du user</param>
        ''' <param name="TelAutre">Autre numéro de téléphone du user</param>
        ''' <param name="Adresse">Adresse du user</param>
        ''' <param name="Ville">Ville du user</param>
        ''' <param name="CodePostal">Code Postal du user</param>
        ''' <returns>ID du user, -1 si erreur, message d'erreur, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveUser(ByVal IdSrv As String, ByVal userId As String, ByVal UserName As String, ByVal Password As String, ByVal Profil As Users.TypeProfil, ByVal Nom As String, ByVal Prenom As String, Optional ByVal NumberIdentification As String = "", Optional ByVal Image As String = "", Optional ByVal eMail As String = "", Optional ByVal eMailAutre As String = "", Optional ByVal TelFixe As String = "", Optional ByVal TelMobile As String = "", Optional ByVal TelAutre As String = "", Optional ByVal Adresse As String = "", Optional ByVal Ville As String = "", Optional ByVal CodePostal As String = "") As String
#End Region

#Region "Device"
        ''' <summary>
        ''' Obtient la liste des devices
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type TemplateDevice, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDevices(ByVal IdSrv As String) As List(Of TemplateDevice)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un device (DeviceID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="DeviceId">ID du device</param>
        ''' <param name="Action">Action (de type Action) à exécutée (ex: "ON", "OFF", "DIM",100...)</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDeviceCommand(ByVal IdSrv As String, ByVal DeviceId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Supprimer un device
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="deviceId">ID du device</param>
        ''' <returns>0 si exécuté, -1 si ID non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDevice(ByVal IdSrv As String, ByVal deviceId As String) As Integer

        ''' <summary>
        ''' Supprime une commande IR d'un device
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="deviceId">ID du device</param>
        ''' <param name="CmdName">Nom de la commande IR</param>
        ''' <returns>0 si exécuté, -1 si ID ou commande non trouvée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDeviceCommandIR(ByVal IdSrv As String, ByVal deviceId As String, ByVal CmdName As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un device par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Id">ID du device</param>
        ''' <returns>Objet de type TemplateDevice, Nothing si non trouvé ou ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDeviceByID(ByVal IdSrv As String, ByVal Id As String) As TemplateDevice

        ''' <summary>
        ''' Retourne une liste de device suivant l'addresse1 et/ou son type et/ou le driver
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="DeviceAdresse">Adresse1 du device</param>
        ''' <param name="DeviceType">Type de device</param>
        ''' <param name="DeviceDriver">ID driver du device</param>
        ''' <returns>List de device, Nothing si non trouvé ou ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDeviceByAdresse1TypeDriver(ByVal IdSrv As String, ByVal DeviceAdresse As String, ByVal DeviceType As String, ByVal DeviceDriver As String, ByVal Enable As Boolean) As ArrayList

        ''' <summary>
        ''' Créer un nouveau device ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="deviceId">ID du device (vide si nouveau, ID du device si modification)</param>
        ''' <param name="name">Nom du device</param>
        ''' <param name="address1">Adresse1 du device</param>
        ''' <param name="enable">Activer/Désactiver le device</param>
        ''' <param name="solo">Indique si le Device travail en mode solo</param>
        ''' <param name="driverid">ID du driver</param>
        ''' <param name="type">Type de device</param>
        ''' <param name="refresh">Valeur de rafraichissement du device</param>
        ''' <param name="address2">Adresse2 du device</param>
        ''' <param name="image">Image du device</param>
        ''' <param name="modele">Modèle de device</param>
        ''' <param name="description">Description du device</param>
        ''' <param name="lastchangeduree">Dernière valeur</param>
        ''' <returns>ID du device créé ou modifié, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDevice(ByVal IdSrv As String, ByVal deviceId As String, ByVal name As String, ByVal address1 As String, ByVal enable As Boolean, ByVal solo As Boolean, ByVal driverid As String, ByVal type As String, ByVal refresh As Integer, Optional ByVal address2 As String = "", Optional ByVal image As String = "", Optional ByVal modele As String = "", Optional ByVal description As String = "", Optional ByVal lastchangeduree As Integer = 0, Optional ByVal lastEtat As Boolean = True, Optional ByVal correction As Double = 0, Optional ByVal formatage As String = "", Optional ByVal precision As Double = 0, Optional ByVal valuemax As Double = 9999, Optional ByVal valuemin As Double = -9999, Optional ByVal valuedef As Double = 0) As String


        ''' <summary>
        ''' Ajouter ou modifier une commande IR à un device (utilisé pour usbuirt)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="deviceId">ID du device</param>
        ''' <param name="CmdName">Nom de la commande IR</param>
        ''' <param name="CmdData">Données IR de la commande</param>
        ''' <param name="CmdRepeat">Nombre de fois à envoyer la commande</param>
        ''' <returns>0 si exécuté, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDeviceCommandIR(ByVal IdSrv As String, ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String

        ''' <summary>
        ''' Commencer l'apprentissage d'un commande IR
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Données de la commande IR, vide si aucune donnée(s) reçue(s), 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function StartIrLearning(ByVal IdSrv As String) As String
#End Region

#Region "Driver"
        ''' <summary>
        ''' Permet de vérifier si un champ a correctement été saisi
        ''' </summary>
        ''' <param name="Idsrv"></param>
        ''' <param name="DriverId"></param>
        ''' <param name="Champ"></param>
        ''' <param name="Value">retourne 0 si Ok sinon le message d'erreur</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function VerifChamp(ByVal Idsrv As String, ByVal DriverId As String, ByVal Champ As String, ByVal Value As Object) As String

        ''' <summary>
        ''' Obtient la liste des drivers
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type TemplateDriver, Nothing si ID du serveur erroné (</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllDrivers(ByVal IdSrv As String) As List(Of TemplateDriver)

        ''' <summary>
        ''' Execute une commande (COMMAND) d'un driver (DriverID) associés à des paramètres (Param)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="DriverId">ID du driver</param>
        ''' <param name="Action">Action de type DeviceAction à exécuter</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub ExecuteDriverCommand(ByVal IdSrv As String, ByVal DriverId As String, ByVal Action As DeviceAction)

        ''' <summary>
        ''' Retourne l'objet d'un driver par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Id">ID du driver</param>
        ''' <returns>List de type TemplateDriver, Nothing si ID non trouvé ou si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByID(ByVal IdSrv As String, ByVal Id As String) As TemplateDriver

        ''' <summary>
        ''' Supprimer un driver de la config
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="driverId">ID du driver à supprimer</param>
        ''' <returns>0 si exécuté, -1 si Driver non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDriver(ByVal IdSrv As String, ByVal driverId As String) As Integer

        ''' <summary>
        ''' Retourne l'objet d'un driver par son nom
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="name">Nom du driver</param>
        ''' <returns>Retourne le driver, Nothing si non trouvé ou ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnDriverByNom(ByVal IdSrv As String, ByVal name As String) As Object

        ''' <summary>
        ''' Modifier un driver 
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="driverId">ID du driver</param>
        ''' <param name="name">Nom du driver</param>
        ''' <param name="enable">Activer/Désactiver le driver</param>
        ''' <param name="startauto">Démarrage automatique du driver</param>
        ''' <param name="iptcp">Adresse IP TCP du driver</param>
        ''' <param name="porttcp">Port TCP du driver</param>
        ''' <param name="ipudp">Adresse IP UDP du driver</param>
        ''' <param name="portudp">Port UDP du driver</param>
        ''' <param name="com">Port COM (RS-232) du driver</param>
        ''' <param name="refresh">Valeur de rafraichissement du driver</param>
        ''' <param name="picture">Image du driver</param>
        ''' <param name="modele">Modele de l'interface</param>
        ''' <param name="Parametres">Paramètres associés au driver</param>
        ''' <returns>ID du driver modifié, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveDriver(ByVal IdSrv As String, ByVal driverId As String, ByVal name As String, ByVal enable As Boolean, ByVal startauto As Boolean, ByVal iptcp As String, ByVal porttcp As String, ByVal ipudp As String, ByVal portudp As String, ByVal com As String, ByVal refresh As Integer, ByVal picture As String, ByVal modele As String, Optional ByVal Parametres As ArrayList = Nothing) As String

        ''' <summary>
        ''' Arrêter un driver
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="DriverId">ID du driver</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub StopDriver(ByVal IdSrv As String, ByVal DriverId As String)

        ''' <summary>
        ''' Démarrer un driver
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="DriverId">ID du driver</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub StartDriver(ByVal IdSrv As String, ByVal DriverId As String)
#End Region

#Region "Zone"
        ''' <summary>
        ''' Indique si la zone ne contient aucun device (exemple à vérifier avant de supprimer une zone)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="zoneId">ID de la zone</param>
        ''' <returns>True si la zone est vide, False si la zone n'est pas vide ou ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ZoneIsEmpty(ByVal IdSrv As String, ByVal zoneId As String) As Boolean

        ''' <summary>
        ''' Supprimer une zone 
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="zoneId">ID de la zone</param>
        ''' <returns>0 si exécuté, -1 si erreur ou non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteZone(ByVal IdSrv As String, ByVal zoneId As String) As Integer

        ''' <summary>
        ''' Obtient la liste des zones
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type Zone, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllZones(ByVal IdSrv As String) As List(Of Zone)

        ''' <summary>
        ''' Retourne la liste des devices d'une zone (par son id)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="zoneId">ID de la zone</param>
        ''' <returns>List de type TemplateDevice, Nothing si zone non trouvée, vide ou si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetDeviceInZone(ByVal IdSrv As String, ByVal zoneId As String) As List(Of TemplateDevice)

        ''' <summary>
        ''' Retourne l'objet d'une zone par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Id">ID de la zone</param>
        ''' <returns>Objet de type Zone, Nothing si zone non trouvée ou si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnZoneByID(ByVal IdSrv As String, ByVal Id As String) As Zone

        ''' <summary>
        ''' Créer un nouveau zone ou sauvegarder la modif (si ID est complété)
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="zoneId">ID de la zone (vide si nouvelle zone, ID de la zone à modifier)</param>
        ''' <param name="name">Nom de la zone</param>
        ''' <param name="ListElement">Liste des éléments (devices, macros, zones) rattachés à la zone</param>
        ''' <param name="icon">Icone de la zone (utilisé pour les boutons)</param>
        ''' <param name="image">Image de la zone (background)</param>
        ''' <returns>ID de la zone créée ou modifiée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveZone(ByVal IdSrv As String, ByVal zoneId As String, ByVal name As String, Optional ByVal ListElement As List(Of Zone.Element_Zone) = Nothing, Optional ByVal icon As String = "", Optional ByVal image As String = "") As String

        ''' <summary>
        ''' Ajouter un device à une zone
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="ZoneId">ID de la zone</param>
        ''' <param name="DeviceId">ID du device à ajouter</param>
        ''' <param name="Visible">Si le device doit être visible ou non côté client</param>
        ''' <returns>0 si exécuté, -1 si zone non trouvée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function AddDeviceToZone(ByVal IdSrv As String, ByVal ZoneId As String, ByVal DeviceId As String, ByVal Visible As Boolean) As String

        ''' <summary>
        ''' Supprimer un device à une zone
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="ZoneId">ID de la zone</param>
        ''' <param name="DeviceId">ID du device à supprimer de la zone</param>
        ''' <returns>0 si exécuté, -1 si zone non trouvée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteDeviceToZone(ByVal IdSrv As String, ByVal ZoneId As String, ByVal DeviceId As String) As String
#End Region

#Region "Macro"

        ''' <summary>
        ''' Supprimer une macro de la config
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="macroId">ID de la macro</param>
        ''' <returns>0 si exécuté, -1 si macro non trouvée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteMacro(ByVal IdSrv As String, ByVal macroId As String) As Integer

        ''' <summary>
        ''' Execute une macro
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Id">ID de la macro</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub RunMacro(ByVal IdSrv As String, ByVal Id As String)

        ''' <summary>
        ''' Retourne la liste de toutes les macros
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type Macro, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract(), ServiceKnownType(GetType(HoMIDom.Macro)), ServiceKnownType(GetType(HoMIDom.Action.ActionDevice)), ServiceKnownType(GetType(HoMIDom.Action.ActionMail)), ServiceKnownType(GetType(HoMIDom.Action.ActionIf)), ServiceKnownType(GetType(HoMIDom.Action.ActionMacro))> Function GetAllMacros(ByVal IdSrv As String) As List(Of Macro)

        ''' <summary>
        ''' Retourne la macro par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="MacroId">ID de la macro</param>
        ''' <returns>Objet de type macro, Nothing si macro non trouvée ou si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract(), ServiceKnownType(GetType(HoMIDom.Macro)), ServiceKnownType(GetType(HoMIDom.Action.ActionDevice)), ServiceKnownType(GetType(HoMIDom.Action.ActionMail)), ServiceKnownType(GetType(HoMIDom.Action.ActionIf)), ServiceKnownType(GetType(HoMIDom.Action.ActionMacro))> Function ReturnMacroById(ByVal IdSrv As String, ByVal MacroId As String) As Macro

        ''' <summary>
        ''' Permet de créer ou modifier une macro
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="macroId">Id de la macro à modifier, mettre une valeur null si c'est une macro à créer</param>
        ''' <param name="nom">Nom de la macro</param>
        ''' <param name="enable">Activation/désactivation de la macro</param>
        ''' <param name="description">Description de la macro</param>
        ''' <param name="listactions">List des actions associées à la macro</param>
        ''' <returns>ID de la macro modifée ou créée, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract(), ServiceKnownType(GetType(HoMIDom.Macro)), ServiceKnownType(GetType(HoMIDom.Action.ActionDevice)), ServiceKnownType(GetType(HoMIDom.Action.ActionMail)), ServiceKnownType(GetType(HoMIDom.Action.ActionIf)), ServiceKnownType(GetType(HoMIDom.Action.ActionMacro))> Function SaveMacro(ByVal IdSrv As String, ByVal macroId As String, ByVal nom As String, ByVal enable As Boolean, Optional ByVal description As String = "", Optional ByVal listactions As ArrayList = Nothing) As String
#End Region

#Region "Trigger"
        ''' <summary>
        ''' Retourne la liste de toutes les triggers
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>List de type Trigger, Nothing si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetAllTriggers(ByVal IdSrv As String) As List(Of Trigger)

        ''' <summary>
        ''' Retourne le trigger par son ID
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="TriggerId">ID du trigger</param>
        ''' <returns>Objet de type Trigger, Nothing si trigger non trouvé ou si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnTriggerById(ByVal IdSrv As String, ByVal TriggerId As String) As Trigger

        ''' <summary>
        ''' Supprimer un trigger 
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="triggerId">ID du trigger</param>
        ''' <returns>0 si exécuté, -1 si trigger non trouvé, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function DeleteTrigger(ByVal IdSrv As String, ByVal triggerId As String) As Integer

        ''' <summary>
        ''' Permet de créer ou modifier un trigger
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="triggerId">ID du trigger (vide si nouveau, ID du trigger pour le modifier)</param>
        ''' <param name="nom">Nom du trigger</param>
        ''' <param name="enable">Activer/Désactiver le trigger</param>
        ''' <param name="TypeTrigger">Type de trigger</param>
        ''' <param name="description">Description du trigger</param>
        ''' <param name="conditiontimer">Condition du trigger s'il est de type Timer</param>
        ''' <param name="deviceid">ID du device à surveiller si trigger de type Device</param>
        ''' <param name="deviceproperty">Propriété du device à surveiller si trigger de type Device</param>
        ''' <param name="macro">Liste des ID des macros associées au trigger</param>
        ''' <returns>ID du trigger créé ou modifié, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function SaveTrigger(ByVal IdSrv As String, ByVal triggerId As String, ByVal nom As String, ByVal enable As Boolean, ByVal TypeTrigger As Trigger.TypeTrigger, Optional ByVal description As String = "", Optional ByVal conditiontimer As String = "", Optional ByVal deviceid As String = "", Optional ByVal deviceproperty As String = "", Optional ByVal macro As List(Of String) = Nothing) As String
#End Region

#Region "Divers"
        ''' <summary>
        ''' Liste les méthodes (actions) dispo pour un device (par son id)
        ''' Retourne pour chaque élément de la liste NOMDELAMETHODE|Parametre1:TypeParametre1|Parametre2:TypeParametre2...
        ''' '' ex pour la classe lampe cela retourne: DIM|Variation:Int32
        ''' </summary>
        ''' <param name="DeviceId">ID du device</param>
        ''' <returns>Retourne pour chaque élément de la liste NOMDELAMETHODE|Parametre1:TypeParametre1|Parametre2:TypeParametre2...</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ListMethod(ByVal DeviceId As String) As List(Of String)

#End Region

#Region "Log"

        ''' <summary>
        ''' Retourne les 4 logs les plus récents (du plus récent au plus ancien)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function Get4Log() As List(Of String)

        ''' <summary>
        ''' Retourne pour chaque type de log s'il doit être pris en compte ou non
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetTypeLogEnable() As List(Of Boolean)

        ''' <summary>
        ''' ''' Fixe si chaque type de log doit être pris en compte ou non
        ''' </summary>
        ''' <param name="ListTypeLogEnable"></param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetTypeLogEnable(ByVal ListTypeLogEnable As List(Of Boolean))

        ''' <summary>
        ''' Retourne le nombre de mois à conserver une archive de log avant de le supprimer
        ''' </summary>
        ''' <param name="Month">Nombre de mois à conserver</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetMaxMonthLog(ByVal Month As Integer)

        ''' <summary>
        ''' Définit le nombre de mois à conserver une archive de log avant de le supprimer
        ''' </summary>
        ''' <returns>Nombre de mois à conserver une archive de log</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetMaxMonthLog() As Integer

        ''' <summary>Ecrit un log dans le fichier log au format xml</summary>
        ''' <param name="TypLog">Type de log ERREUR, MESSAGE, INFO...</param>
        ''' <param name="Source">Source du log CLIENT, SERVEUR...</param>
        ''' <param name="Fonction">Nom de la fonction ayant déclenchée le log</param>
        ''' <param name="Message">Message du log</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub Log(ByVal TypLog As HoMIDom.Server.TypeLog, ByVal Source As HoMIDom.Server.TypeSource, ByVal Fonction As String, ByVal Message As String)

        ''' <summary>
        ''' Renvoi le fichier log suivant une requête xml si besoin
        ''' </summary>
        ''' <param name="Requete"></param>
        ''' <returns>Log</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function ReturnLog(Optional ByVal Requete As String = "") As String

        ''' <summary>
        ''' Fixe la taille max du fichier log en Ko avant d'en créer un nouveau
        ''' </summary>
        ''' <param name="Value">Taille maximale du fichier log en Ko</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetMaxFileSizeLog(ByVal Value As Long)

        ''' <summary>
        ''' Retourne la taille max du fichier log en Ko
        ''' </summary>
        ''' <returns>Taille maximale du fichier log en Ko</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetMaxFileSizeLog() As Long
#End Region

#Region "SMTP"
        ''' <summary>
        ''' Retourne l'adresse SMTP du serveur
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Adresse SMTP du serveur, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPServeur(ByVal IdSrv As String) As String

        ''' <summary>
        ''' Fixe l'adresse SMTP du serveur
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Adresse SMTP du serveur</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPServeur(ByVal IdSrv As String, ByVal Value As String)

        ''' <summary>
        ''' Retourne le login du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Login du serveur SMTP, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPLogin(ByVal IdSrv As String) As String

        ''' <summary>
        ''' Fixe le login du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Login du serveur SMTP</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPLogin(ByVal IdSrv As String, ByVal Value As String)

        ''' <summary>
        ''' Retourne le password du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Pasword du serveur SMTP, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPPassword(ByVal IdSrv As String) As String

        ''' <summary>
        ''' Fixe le password du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Password du serveur SMTP</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPPassword(ByVal IdSrv As String, ByVal Value As String)

        ''' <summary>
        ''' Retourne l'adresse mail du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <returns>Adresse mail du serveur SMTP, 99 si ID du serveur erroné</returns>
        ''' <remarks></remarks>
        <OperationContract()> Function GetSMTPMailServeur(ByVal IdSrv As String) As String

        ''' <summary>
        ''' Fixe l'adresse mail du serveur SMTP
        ''' </summary>
        ''' <param name="IdSrv">L'ID du serveur doit être passé en paramètre pour exécuter cette fonction</param>
        ''' <param name="Value">Adresse mail du serveur SMTP</param>
        ''' <remarks></remarks>
        <OperationContract()> Sub SetSMTPMailServeur(ByVal IdSrv As String, ByVal Value As String)
#End Region

    End Interface

End Namespace