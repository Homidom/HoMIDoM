﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Ce code a été généré par un outil.
'     Version du runtime :4.0.30319.1008
'
'     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
'     le code est régénéré.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    'à l'aide d'un outil, tel que ResGen ou Visual Studio.
    'Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    'avec l'option /str ou régénérez votre projet VS.
    '''<summary>
    '''  Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("WpfApplication1.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Remplace la propriété CurrentUICulture du thread actuel pour toutes
        '''  les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        Friend ReadOnly Property Defaut_Img() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("Defaut_Img", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Recherche une chaîne localisée semblable à &apos;EXEMPLE *******************************************
        '''&apos;***************************************************
        '''Imports System
        '''Imports System.IO
        '''Imports System.Windows.Forms
        '''Imports Microsoft.VisualBasic
        '''Imports Homidom                                                                  
        '''Namespace Dynam
        ''' &apos;Ne pas supprimer cette class
        ''' Public Class DynamicCode
        '''   
        '''  &apos;Ne pas supprimer cette function 
        '''  Public Function ExecuteCode(paramarray prmParameters() as object) as object
        '''    Dim Serveur as Homidom.H [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Friend ReadOnly Property ExempleVBS() As String
            Get
                Return ResourceManager.GetString("ExempleVBS", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Recherche une chaîne localisée semblable à FRXX0001;Aix-en-Provence
        '''FRXX0002;Albi
        '''FRXX0003;Annemasse
        '''FRXX0004;Antibes
        '''FRXX0005;Arnage
        '''FRXX0006;Aubagne
        '''FRXX0007;Aubervilliers
        '''FRXX0008;Auxerre
        '''FRXX0009;Aytre
        '''FRXX0010;Bayeux
        '''FRXX0011;Beauvais
        '''FRXX0012;Belfort
        '''FRXX0013;Beziers
        '''FRXX0014;Blois
        '''FRXX0015;Bobigny
        '''FRXX0016;Bordeaux
        '''FRXX0017;Bourg-en-Bresse
        '''FRXX0018;Bourges
        '''FRXX0019;Brest
        '''FRXX0020;Caen
        '''FRXX0021;Cagnes-sur-Mer
        '''FRXX0022;Cahors
        '''FRXX0023;Cannes
        '''FRXX0024;Carbon-Blanc
        '''FRXX0025;Carcassonne
        '''FRXX0026;Carquefou
        '''FRXX0027;Casteln [le reste de la chaîne a été tronqué]&quot;;.
        '''</summary>
        Friend ReadOnly Property WeatherCityID() As String
            Get
                Return ResourceManager.GetString("WeatherCityID", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
