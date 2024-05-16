# Dependency Injection

PDMSDK provides some support for dependency injection (DI). The Container property in the AddInBase allows to resolve your types into instances. 

We use [Simple Injector](https://simpleinjector.org/).

# Type registration

- Override the method [OnRegisterAdditionalTypes](../api/BlueByte.SOLIDWORKS.PDMProfessional.SDK.AddInBase.html#BlueByte_SOLIDWORKS_PDMProfessional_SDK_AddInBase_OnRegisterAdditionalTypes_SimpleInjector_Container_). Example:

# [C Sharp](#tab/cs)
```
protected override void OnRegisterAdditionalTypes(Container container)
        {
            base.OnRegisterAdditionalTypes(container);
            container.Register<ISOLIDWORKSInstanceManager, SOLIDWORKSInstanceManager>();
            container.RegisterSingleton<IEvaluationService, EvaluationService>();
            container.RegisterSingleton <ISettingsManager,SettingsManager>();
        }
```
# [VB](#tab/VB)
```
Protected Overrides Sub OnRegisterAdditionalTypes(container As Container)
    MyBase.OnRegisterAdditionalTypes(container)
    container.Register(Of ISOLIDWORKSInstanceManager, SOLIDWORKSInstanceManager)()
    container.RegisterSingleton(Of IEvaluationService, EvaluationService)()
    container.RegisterSingleton(Of ISettingsManager, SettingsManager)()
End Sub
```
---

- You can call the Container property anywhere else in your AddInBase implementation and resolve a registered type to get an instance. Example:

# [C Sharp](#tab/cs)
```
var settingsManager = Container.GetInstance<ISettingsManager>();
```
# [VB](#tab/VB)
```
Dim settingsManager As ISettingsManager = Container.GetInstance(Of ISettingsManager)()

```
---