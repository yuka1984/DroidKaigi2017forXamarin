# DroidKaigi2017forXamarin

https://github.com/DroidKaigi/conference-app-2017

を元にXamarinで一部機能を再現してみました。

# Nyanto Framwwork

## コンセプト

Xamarin AndroidでAndroidアプリを作る際に @yuka1984が楽できるために作成しているフレームワークです。

Xamarin AndroidでAutofacによるDIとReactivePropertyの利用を助けます。

またLifeCycleへの対応をReactivePropertyと関連して実現しようとしています。

## 経緯
Xamarin Androidを行うならできる限りネイティブと同じライブラリを用いるのが良いのだろうと思っていました。
けれど、今回DoroidoKaigi2017アプリをXamarin/C#に移植しようとしている中で、言語使用、周辺のTool、文化、がかなり違う中で、
それでもAndroid/Javaの環境とできる限り同じように作っていくよりも、Xamarin AndroidをAndroidネイティブとは異なる
全然違う環境でのアプリ作成ととらえて.NETのエコシステムの中で作成した方が幸せなんじゃないか？　みたいな事をふと思い立って
じゃあ、私が幸せになれるフレームワークを作ってみよう、という事で作成を開始しました。

普通に考えたら、絶対にArchitectureComponentベースにデザインしていった方が幸せだと思います。

## DI

DroidKaigiはDIにDagger2が使われていてStilettoが用意されていてAndroidアプリの開発に近づけるのであればStilettoを用いたほうが良いのだけど、私は.NETでよく使われているDIライブラリを使いたかったのでAutofacを用いてDIできるようにした。

Droidkaigiアプリとは異なるDIとライフサイクル管理をしています。

## 使い方

### ApplicationBase

まずはApplicationBaseを継承したApplicationクラスを作成します。

ApplicationBaseはabstractでContainerSettingメソッドの実装を行う必要があります。

```
protected abstract void ContainerSetting(ContainerBuilder builder)
```

このメソッドでAutofac.ContainerBuilderでDIの設定を行います。

Applicationクラスと同じライフサイクルを維持したい場合はSingleInstance

Activetyクラスと同じライフサイクルを維持したい場合はInstancePerLifetimeScope

とします。

### AppCompatActivityBase

次にActivityを作る際にはAppCompatActivityBaseを継承して作成します。

AppCompatActivityBaseはApplicationBaseを継承して作成したApplicationクラスよりAutofacのコンテナを取得し、BeginScopeを行ってILifetimeScopeを作成します。

この作成されたILifetimeScopeはArchitectureComponentsのViewModelと同じくHolderFragmentを使用して保持されます。
これによってILifetimeScopeはActicityのLifecycleを超えて維持されActivityが完全に利用されなくなった時点でDisposeが行われて終了します。

AppCompatActivityBaseを継承したクラスは自動的にPropertyInjectionが行われます。
コンテナに登録してあるクラスが必要であればPropertyとして実装しておくとインジェクションされます。
Injectionが行われるタイミングはOnCreateです。base.OnCreateが呼び出される前はインジェクションされていません。

AppCompatActivityBaseを継承したクラスはConfigurationActionメソッドを実装する必要があります。

```
protected abstract void ConfigurationAction(ContainerBuilder containerBuilder);
```

そのActivetyの範囲でインジェクションを行いたい場合に、ここでBuilderに登録します。主にActivity自身のインスタンスが必要なクラスなどを登録する事を想定しています。

### FragmentBase

次にFragmentを作る際にはFragmentBaseを継承して作成します。

FragmentBaseにはGeneric版

``` public abstract class FragmentBase<T> : FragmentBase where T : ViewModelBase```

があり、ほとんどのケースでそちらを利用します。

FragmentBase<T> は T のプロパティ ViewModelを持ちます。プロパティですのでAutofacにGenericに指定したViewModelが登録されていればインジェクションが自動的に行われます。

ViewModelbaseは後述しますが、多くの場合ViewModelはAutofacのInstancePerLifetimeScopeで登録を行いますのでアクティビティが存在している間(インスタンスサイクルではない)は同じインスタンスが保持されます。

要するにFragmentが何回再生成されようが同じインスタンスのViewModelがインジェクションされる、という事です。

FragmentBaseを継承したクラスは

``` public abstract int ViewResourceId { get; } ```

と

``` protected abstract void Bind(View view); ```

を実装する必要があります。

ViewResoureIdはFragmentのViewLayoutのリソースIDを返答してください。

Bind関数ではviewが渡されてViewModelとViewのBindを行うことを想定しています。

BindはFragmentのOnCreateView中にコールされます。

FragmentBaseはCompositDisposableフィールドを持っています。

Reactiveを使用する場合に、SubscrbeしたIDisposableはCompositDisposableに入れておけばFragment.OnDetachのタイミングでDisposeが行われて購読の解除を行うことができます。


