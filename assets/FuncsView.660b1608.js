import{Q as r}from"./QImg.a522ac1f.js";import{i as b,am as y,s as B,t as E,v as c,x as t,d as e,y as u,an as f,ao as g,aj as p,z as O,ai as h,_ as v,r as q,g as P,ap as H,A as j,B as k,ah as C,aq as A,C as s,ag as R,ak as I,F as w}from"./index.fb862ff1.js";function K(){return b(y)}const M={class:"absolute-bottom text-h6"},x={class:"absolute-center fit"},Q=B({__name:"FuncItem",props:{assetPath:{},title:{},dialogComponent:{type:[Object,Function,String]}},setup(l){const o=l,F=K();function n(){F.dialog({component:o.dialogComponent,componentProps:{title:o.title,assetPath:o.assetPath}})}return(a,_)=>(E(),c(h,{class:"card"},{default:t(()=>[e(r,{src:a.assetPath},{default:t(()=>[u("div",M,f(a.title),1)]),_:1},8,["src"]),e(p,null,{default:t(()=>[g(a.$slots,"default",{},void 0,!0)]),_:3}),u("div",x,[e(O,{color:"transparent",class:"fit",onClick:n})])]),_:3}))}});var D=v(Q,[["__scopeId","data-v-2f6a8adc"]]),T="/ServerStarter/assets/ServerBootBtn.7c697a8a.png",W="/ServerStarter/assets/Property.7eaccfa1.png",V="/ServerStarter/assets/Players.d5500a43.png";const N=()=>!0;function z(l){const o={};return l.forEach(F=>{o[F]=N}),o}function i(){const{emit:l,proxy:o}=P(),F=q(null);function n(){F.value.show()}function a(){F.value.hide()}function _($){l("ok",$),a()}function m(){l("hide")}return Object.assign(o,{show:n,hide:a}),{dialogRef:F,onDialogHide:m,onDialogOK:_,onDialogCancel:a}}const S=["ok","hide"];i.emits=S;i.emitsObject=z(S);const G=l=>(j("data-v-7240ea8c"),l=l(),k(),l),J=G(()=>u("span",null,"\u25A0\u3084\u308A\u65B9",-1)),L=B({__name:"BaseDialogCard",props:{title:{},color:{},onClose:{type:Function}},setup(l){return(o,F)=>(E(),c(h,{flat:"",class:"q-py-sm"},{default:t(()=>[e(p,{class:"q-pt-xs"},{default:t(()=>[u("div",{class:H(["title",o.color!==void 0?`text-${o.color}`:""])},f(o.title),3)]),_:1}),e(p,{class:"q-pt-none"},{default:t(()=>[g(o.$slots,"default",{},void 0,!0)]),_:3}),e(p,{class:"q-pt-none"},{default:t(()=>[J,g(o.$slots,"install",{},void 0,!0)]),_:3})]),_:3}))}});var d=v(L,[["__scopeId","data-v-7240ea8c"]]);const U=u("p",null,"\u8907\u96D1\u306A\u30B5\u30FC\u30D0\u30FC\u306E\u4ED5\u7D44\u307F\u3084\u8A2D\u5B9A\u306E\u96E3\u3057\u3055\u306F\u4E00\u5207\u3042\u308A\u307E\u305B\u3093",-1),X=u("p",null,"\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3059\u308B\u306B\u306F\uFF0C\u8D77\u52D5\u30DC\u30BF\u30F3\u3092\u30AF\u30EA\u30C3\u30AF\u3059\u308B\u3060\u3051\u3067\u3059",-1),Y=u("p",null,[s(" \u6163\u308C\u3066\u304D\u3066\u8A2D\u5B9A\u3092\u30A2\u30EC\u30F3\u30B8\u3057\u305F\u3044\u6642\u306B\uFF0C\u96E3\u3057\u3044\u3053\u3068\u306F\u4F55\u3082\u3042\u308A\u307E\u305B\u3093"),u("br"),s(" \u8A73\u7D30\u306A\u8A2D\u5B9A\u3092\u30B5\u30DD\u30FC\u30C8\u3059\u308B\u898B\u3084\u3059\u3044\u753B\u9762\u3092\u305F\u304F\u3055\u3093\u3054\u7528\u610F\u3057\u3066\u304A\u308A\u307E\u3059 ")],-1),Z={class:"column q-gutter-md q-pr-md"},uu=B({__name:"EasyOperationDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{default:t(()=>[U,X,e(r,{src:T,class:"q-mb-md"}),Y,u("div",Z,[e(r,{src:W}),e(r,{src:V})])]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}});var eu="/ServerStarter/assets/servers.a0da1650.png";const tu=u("p",null,"ServerStarter\u3067\u306F6\u7A2E\u985E\u306E\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059",-1),su=u("ul",null,[u("li",null,"\u30D0\u30CB\u30E9\uFF08Minecraft\u306E\u516C\u5F0F\u30B5\u30FC\u30D0\u30FC\uFF09"),u("li",null,"Spigot"),u("li",null,"PaperMC"),u("li",null,"Fabric"),u("li",null,"Forge"),u("li",null,"MohistMC")],-1),ou=u("p",null,[s(" \u30B5\u30FC\u30D0\u30FC\u306E\u7A2E\u985E\u3092\u5F8C\u304B\u3089\u5909\u66F4\u3059\u308B\u3053\u3068\u3082\u53EF\u80FD\uFF01"),u("br"),s(" \u69D8\u3005\u306A\u904A\u3073\u65B9\u306B\u67D4\u8EDF\u306B\u5BFE\u5FDC\u3067\u304D\u307E\u3059 ")],-1),Fu=B({__name:"MultipleServersDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{default:t(()=>[tu,su,ou,e(r,{src:eu})]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}});var au="/ServerStarter/assets/PlayerSettings.a2c6eb5d.png",lu="/ServerStarter/assets/Groups.61b8b7df.png",ru="/ServerStarter/assets/tabs.31d15af9.png",nu="/ServerStarter/assets/PlayerOperation.ab19b763.png",iu="/ServerStarter/assets/GroupOperation.9ef02060.png";const Cu=u("p",null,[s(" \u96E3\u3057\u3044\u30B3\u30DE\u30F3\u30C9\u3092\u899A\u3048\u305F\u308A\uFF0C\u8907\u96D1\u306A\u30D5\u30A1\u30A4\u30EB\u3092\u7DE8\u96C6\u3057\u305F\u308A\u3059\u308B\u4F5C\u696D\u306B\u5225\u308C\u3092\u544A\u3052\u307E\u3057\u3087\u3046"),u("br"),s(" ServerStarter\u3067\u306F\uFF0COP\u6A29\u9650\u3084\u30DB\u30EF\u30A4\u30C8\u30EA\u30B9\u30C8\u3068\u8A00\u3063\u305F\uFF0C\u30D7\u30EC\u30A4\u30E4\u30FC\u306E\u6A29\u9650\u3092\u7C21\u5358\u306B\u8A2D\u5B9A\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059 ")],-1),Bu=u("p",null," \u30E6\u30FC\u30B6\u30FC\u306E\u4E00\u89A7\u3068\u6A29\u9650\u306E\u69D8\u5B50\u306F\u8996\u899A\u7684\u306B\u5206\u304B\u308A\u3084\u3059\u304F\u8868\u793A\u3055\u308C\u308B\u3088\u3046\u306B\u306A\u308A\u307E\u3059\uFF01 ",-1),Eu=u("p",null,[s(" \u8907\u6570\u306E\u30E6\u30FC\u30B6\u30FC\u3092\u767B\u9332\u3059\u308B\u3068\u304D\u306B\u306F\uFF0C\u30B0\u30EB\u30FC\u30D7\u6A5F\u80FD\u304C\u4FBF\u5229\u3067\u3059"),u("br"),s(" \u4E00\u62EC\u3067\u30DB\u30EF\u30A4\u30C8\u30EA\u30B9\u30C8\u306B\u767B\u9332\u3057\u305F\u308A\uFF0COP\u6A29\u9650\u3092\u8A2D\u5B9A\u3057\u305F\u308A\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059\uFF01 ")],-1),_u=u("li",null,"\u30D7\u30EC\u30A4\u30E4\u30FC\u30BF\u30D6\u3092\u9078\u629E",-1),cu=u("li",null,"\u30D7\u30EC\u30A4\u30E4\u30FC\u4E00\u89A7\u3092\u66F4\u65B0",-1),Du=u("ul",null,[u("li",null,"\u767B\u9332\u3057\u305F\u3044\u30D7\u30EC\u30A4\u30E4\u30FC\u540D\u3092\u5165\u529B\u6B04\u3067\u691C\u7D22"),u("li",null,"\u30B0\u30EB\u30FC\u30D7\u304B\u3089\u4E00\u62EC\u3067\u8FFD\u52A0"),u("li",null,"\u500B\u3005\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u3084\uFF0C\u30B0\u30EB\u30FC\u30D7\u3067\u9078\u629E\u3057\u305F\u30D7\u30EC\u30A4\u30E4\u30FC\u3092\u524A\u9664")],-1),Au={class:"row q-gutter-md q-pr-md q-pt-md justify-center"},du=B({__name:"PlayerDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{install:t(()=>[u("ol",null,[_u,e(r,{src:ru,class:"q-mb-md"}),cu,Du,s(" \u53F3\u5074\u306E\u64CD\u4F5C\u76E4\u304B\u3089\u69D8\u3005\u306A\u8A2D\u5B9A\u3092\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059\uFF01 "),u("div",Au,[e(r,{src:nu,style:{width:"45%"}}),e(r,{src:iu,style:{width:"45%"}})])])]),default:t(()=>[Cu,Bu,e(r,{src:au,class:"q-mb-md"}),Eu,e(r,{src:lu})]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}});var pu="/ServerStarter/assets/importCustomMapBtn.97377f64.png",mu="/ServerStarter/assets/singleMaps.94cc649b.png";const gu=u("ul",null,[u("li",null,"\u4E00\u4EBA\u3067\u904A\u3093\u3067\u3044\u305F\u30EF\u30FC\u30EB\u30C9\u3092\u307F\u3093\u306A\u3068\u30B7\u30A7\u30A2\u3057\u305F\u3044"),u("li",null,"\u30EF\u30FC\u30EB\u30C9\u88FD\u4F5C\u3092\u307F\u3093\u306A\u306B\u5354\u529B\u3057\u3066\u3082\u3089\u3044\u305F\u3044")],-1),fu=u("p",null,[s(" ServerStarter\u3067\u306F\u500B\u4EBA\u30EF\u30FC\u30EB\u30C9\u3092\u7C21\u5358\u306B\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u5316\u3059\u308B\u6A5F\u80FD\u3092\u642D\u8F09\uFF01"),u("br"),s(" \u30EF\u30FC\u30EB\u30C9\u306E\u5171\u6709\u306B\u96E3\u3057\u3044\u64CD\u4F5C\u306F\u5FC5\u8981\u3042\u308A\u307E\u305B\u3093\uFF01 ")],-1),hu=u("li",null,"\u30DB\u30FC\u30E0\u30BF\u30D6\u306B\u3042\u308B\u300C\u65E2\u5B58\u30EF\u30FC\u30EB\u30C9\u3092\u9078\u629E\u300D\u30DC\u30BF\u30F3\u3092\u30AF\u30EA\u30C3\u30AF",-1),vu=u("li",null,"\u30B7\u30F3\u30B0\u30EB\u30D7\u30EC\u30A4\u306E\u30EF\u30FC\u30EB\u30C9\u4E00\u89A7\u304B\u3089\u597D\u304D\u306A\u30EF\u30FC\u30EB\u30C9\u3092\u9078\u629E\u3059\u308B\u3060\u3051\uFF01",-1),Su=B({__name:"SinglePlayDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{install:t(()=>[u("ol",null,[hu,e(r,{src:pu,class:"q-mb-md"}),vu,e(r,{src:mu})])]),default:t(()=>[gu,fu]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}});var $u="/ServerStarter/assets/BeforeAdding.7c1c1d04.png",bu="/ServerStarter/assets/EditDialog.47cc4f14.png",yu="/ServerStarter/assets/AfterAdding.a82458a4.png";const Ou=u("ul",null,[u("li",null,"\u30EF\u30FC\u30EB\u30C9\u306E\u4FDD\u5B58\u5834\u6240\u304C\u308F\u304B\u3089\u306A\u3044"),u("li",null,"\u76F4\u63A5\u7DE8\u96C6\u3059\u308B\u305F\u3081\u306B\u898B\u3084\u3059\u3044\u5834\u6240\u306B\u4FDD\u5B58\u3057\u3066\u304A\u304D\u305F\u3044")],-1),qu=u("p",null,"ServerStarter\u3067\u306F\u4FDD\u5B58\u5834\u6240\u306E\u5909\u66F4\u3082\u300C\u30EF\u30F3\u30AF\u30EA\u30C3\u30AF\u300D\u3067\u884C\u3046\u3053\u3068\u304C\u3067\u304D\u307E\u3059\uFF01",-1),Pu=u("p",null,[s(" \u4FDD\u5B58\u5834\u6240\u3054\u3068\u306B\u597D\u304D\u306A\u540D\u524D\u3092\u3064\u3051\u305F\u308A\uFF0C\u8868\u793A\u975E\u8868\u793A\u3092\u5207\u308A\u66FF\u3048\u305F\u308A\u3067\u304D\u307E\u3059"),u("br"),s(" \u30EF\u30FC\u30EB\u30C9\u306E\u7BA1\u7406\u3092\u3088\u308A\u7C21\u5358\u306B\u3059\u308B\u6A5F\u80FD\u3067\u3059\uFF01 ")],-1),Hu=u("li",null,"\u300C\u30DB\u30FC\u30E0\u300D\u30BF\u30D6\u306E\u300C\u30EF\u30FC\u30EB\u30C9\u30D5\u30A9\u30EB\u30C0\u300D\u3092\u958B\u304F",-1),ju=u("li",null,"\u300C\u30EF\u30FC\u30EB\u30C9\u30D5\u30A9\u30EB\u30C0\u3092\u8FFD\u52A0\u300D\u30DC\u30BF\u30F3\u3092\u30AF\u30EA\u30C3\u30AF\u3057\uFF0C\u597D\u304D\u306A\u540D\u524D\u306E\u8A2D\u5B9A\u3068\uFF0C\u8A2D\u5B9A\u3059\u308B\u30D5\u30A9\u30EB\u30C0\u3092\u9078\u629E",-1),ku=u("li",null,"\u8FFD\u52A0\u3055\u308C\u305F\u30A2\u30A4\u30C6\u30E0\u3092\u9078\u629E\u3059\u308B\u3053\u3068\u3067\uFF0C\u4FDD\u5B58\u5834\u6240\u3092\u6C7A\u3081\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059",-1),Ru=B({__name:"WorldFolderDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{install:t(()=>[u("ol",null,[Hu,e(r,{src:$u,class:"q-mb-md"}),ju,e(r,{src:bu,class:"q-mb-md"}),ku,e(r,{src:yu})])]),default:t(()=>[Ou,qu,Pu]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}});var Iu="/ServerStarter/assets/BeforeSetting.57507b0e.png",wu="/ServerStarter/assets/AfterSetting.17cd3b10.png";const Ku=u("p",null,[s(" \u500B\u4EBA\u3067\u30B5\u30FC\u30D0\u30FC\u3092\u7ACB\u3066\u3066\u53CB\u4EBA\u3068\u904A\u3076\u969B\u306B\uFF0C\u30B5\u30FC\u30D0\u30FC\u3092\u7ACB\u3066\u305F\u4EBA\u304C\u3044\u306A\u3044\u3068\u53CB\u4EBA\u3060\u3051\u3067\u306F\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u30EF\u30FC\u30EB\u30C9\u306B\u5165\u308C\u307E\u305B\u3093\u3067\u3057\u305F\uFF0E"),u("br"),s(" \u30C7\u30FC\u30BF\u3092\u5171\u6709\u3059\u308B\u306B\u3082\uFF0C\u904A\u3073\u7D42\u308F\u3063\u305F\u5F8C\u306B\u5171\u6709\u4F5C\u696D\u3092\u3059\u308B\u306E\u306F\u975E\u5E38\u306B\u9762\u5012\u3067\u3059 ")],-1),Mu=u("p",null," ServerStarter\u306EShareWorld\u3092\u4F7F\u3048\u3070\uFF0C\u7C21\u5358\u306B\u30B5\u30FC\u30D0\u30FC\u30C7\u30FC\u30BF\u3092\u8907\u6570\u4EBA\u3067\u5171\u6709\u3057\uFF0C \u5171\u6709\u76F8\u624B\u306F\u3044\u3064\u3067\u3082\u6700\u65B0\u306E\u30EF\u30FC\u30EB\u30C9\u3092\u7ACB\u3061\u4E0A\u3052\u308B\u3053\u3068\u304C\u3067\u304D\u308B\u3088\u3046\u306B\u306A\u308A\u307E\u3059\uFF01 ",-1),xu=u("p",null," \u30D7\u30EC\u30A4\u30E4\u30FC\u9593\u3067\u81EA\u7531\u306B\u30B5\u30FC\u30D0\u30FC\u3092\u7ACB\u3061\u4E0A\u3052\u3066\uFF0C\u3044\u3064\u3067\u3082\u6700\u65B0\u306E\u30EF\u30FC\u30EB\u30C9\u3092\u30D7\u30EC\u30A4\u3057\u307E\u3057\u3087\u3046\uFF01 ",-1),Qu={class:"column q-gutter-md q-mr-md"},Tu=B({__name:"ShareWorldDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>{const m=R("router-link");return E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{default:t(()=>[Ku,Mu,xu,u("p",null,[s(" \u4E8B\u524D\u6E96\u5099\u3084\u4F7F\u3044\u65B9\u306E\u8A73\u7D30\u306F\uFF0C "),e(m,{class:"a",to:"/ShareWorld"},{default:t(()=>[s("ShareWorld\u306E\u89E3\u8AAC\u8A18\u4E8B")]),_:1}),s(" \u3092\u3054\u89A7\u304F\u3060\u3055\u3044\uFF01 ")]),u("div",Qu,[e(r,{src:Iu}),e(r,{src:wu})])]),_:1},8,["title","onClose"])]),_:1},8,["onHide"])}}});var Wu="/ServerStarter/assets/defaultIcons.c2d2e754.png",Vu="/ServerStarter/assets/CustomImg.a851c0aa.png";const Nu=u("p",null,[s(" \u30B5\u30FC\u30D0\u30FC\u306E\u53C2\u52A0\u8005\u304C\u8907\u6570\u306E\u30B5\u30FC\u30D0\u30FC\u3092\u898B\u5206\u3051\u308B\u969B\u306B\u91CD\u8981\u8996\u3059\u308B\u8981\u7D20\u306E\u4E00\u3064\u306B\u300C\u30B5\u30FC\u30D0\u30FC\u30A2\u30A4\u30B3\u30F3\u300D\u304C\u3042\u308A\u307E\u3059"),u("br"),s(" \u9069\u5207\u306A\u30B5\u30FC\u30D0\u30FC\u30A2\u30A4\u30B3\u30F3\u306E\u8A2D\u5B9A\u306F\u30B5\u30FC\u30D0\u30FC\u306E\u69D8\u5B50\u3092\u4E00\u76EE\u3067\u78BA\u8A8D\u3067\u304D\u308B\u4E00\u65B9\uFF0C\u9069\u5207\u306A\u753B\u50CF\u3092\u81EA\u524D\u3067\u7528\u610F\u3059\u308B\u306E\u306F\u9762\u5012\u3067\u3057\u305F ")],-1),zu=u("p",null,[s(" ServerStarter\u3067\u306F\u30B5\u30FC\u30D0\u30FC\u3092\u3088\u308A\u30D6\u30E9\u30C3\u30B7\u30E5\u30A2\u30C3\u30D7\u3059\u308B\u305F\u3081\u306E\u30B5\u30DD\u30FC\u30C8\u3092\u60DC\u3057\u307F\u307E\u305B\u3093\uFF01"),u("br"),s(" \u81EA\u7531\u306A\u753B\u50CF\u3092\u9069\u5207\u306A\u753B\u50CF\u306B\u81EA\u52D5\u3067\u5909\u63DB\u3059\u308B\u3060\u3051\u3067\u306A\u304F\uFF0C\u7C21\u5358\u306B\u8A2D\u5B9A\u3059\u308B\u305F\u3081\u306E\u30A2\u30A4\u30B3\u30F3\u5019\u88DC\u3082\u5B8C\u5099\uFF01"),u("br"),s(" \u81EA\u5206\u306E\u30B5\u30FC\u30D0\u30FC\u3092\u660E\u308B\u304F\u7740\u98FE\u308A\u307E\u3057\u3087\u3046\uFF01 ")],-1),Gu=u("ol",null,[u("li",null,"\u30DB\u30FC\u30E0\u30BF\u30D6\u53F3\u4E0A\u306E\u300C\u30A2\u30A4\u30B3\u30F3\u3092\u5909\u66F4\u300D\u30DC\u30BF\u30F3\u3092\u30AF\u30EA\u30C3\u30AF"),u("li",null,"\u304A\u597D\u304D\u306A\u30D6\u30ED\u30C3\u30AF\u3092\u30AF\u30EA\u30C3\u30AF\u3059\u308B\u304B\uFF0C\u5DE6\u4E0A\u306E\u300C\uFF0B\u300D\u30DC\u30BF\u30F3\u304B\u3089\u597D\u304D\u306A\u753B\u50CF\u3092\u9078\u629E\u3059\u308B\u3060\u3051\uFF01")],-1),Ju={class:"column q-gutter-md q-pr-md"},Lu=B({__name:"ServerIconDialog",props:{assetPath:{},title:{},color:{}},emits:{...i.emitsObject},setup(l){const{dialogRef:o,onDialogHide:F,onDialogOK:n}=i();return(a,_)=>(E(),c(A,{ref_key:"dialogRef",ref:o,onHide:C(F)},{default:t(()=>[e(d,{title:a.title,onClose:C(n)},{install:t(()=>[Gu,u("div",Ju,[e(r,{src:Wu}),e(r,{src:Vu})])]),default:t(()=>[Nu,zu]),_:1},8,["title","onClose"])]),_:1},8,["onHide"]))}}),Uu=u("h1",{class:"q-pl-md"},"\u4E3B\u8981\u6A5F\u80FD\u4E00\u89A7",-1),Xu={class:"row q-gutter-md q-pa-md justify-center"},u0=B({__name:"FuncsView",setup(l){return(o,F)=>(E(),I(w,null,[Uu,u("div",Xu,[e(D,{title:"6\u7A2E\u985E\u306E\u30B5\u30FC\u30D0\u30FC\u306B\u5BFE\u5FDC","asset-path":"~assets/Funcs/MultipleServers/Top.png","dialog-component":Fu},{default:t(()=>[s(" \u53B3\u9078\u3057\u305F6\u7A2E\u985E\u306E\u30B5\u30FC\u30D0\u30FC\u304C\u69D8\u3005\u306A\u904A\u3073\u65B9\u3092\u30B5\u30DD\u30FC\u30C8\u3057\u307E\u3059 ")]),_:1}),e(D,{title:"\u500B\u4EBA\u30EF\u30FC\u30EB\u30C9\u306B\u3054\u62DB\u5F85","asset-path":"~assets/Funcs/SinglePlay/Top.png","dialog-component":Su},{default:t(()=>[s(" \u30B7\u30F3\u30B0\u30EB\u30D7\u30EC\u30A4\u30EF\u30FC\u30EB\u30C9\u3092\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u5316\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059 ")]),_:1}),e(D,{title:"\u76F4\u611F\u7684\u306A\u5206\u304B\u308A\u3084\u3059\u3055","asset-path":"~assets/Funcs/EasyOperation/Top.png","dialog-component":uu},{default:t(()=>[s(" \u5404\u7A2E\u8A2D\u5B9A\u3001\u30C7\u30FC\u30BF\u30D1\u30C3\u30AF\u306A\u3069\u3092\u8996\u899A\u7684\u306B\u308F\u304B\u308A\u3084\u3059\u3044\u64CD\u4F5C\u3067\u5C0E\u5165\u3067\u304D\u308B\u3088\u3046\u306B\u3053\u3060\u308F\u308A\u307E\u3057\u305F ")]),_:1}),e(D,{title:"\u4FDD\u5B58\u5834\u6240\u306F\u81EA\u7531\u81EA\u5728","asset-path":"https://cdn.quasar.dev/img/parallax2.jpg","dialog-component":Ru},{default:t(()=>[s(" \u4FDD\u5B58\u5834\u6240\u306E\u5909\u66F4\u3082\u300C\u30EF\u30F3\u30AF\u30EA\u30C3\u30AF\u300D\u3067\u3059\u3050\u306B\u53CD\u6620\u3059\u308B\u3053\u3068\u304C\u3067\u304D\u307E\u3059 ")]),_:1}),e(D,{title:"\u30D7\u30EC\u30A4\u30E4\u30FC\u6A29\u9650\u8A2D\u5B9A","asset-path":"~assets/Funcs/Player/Top.png","dialog-component":du},{default:t(()=>[s(" OP\u6A29\u9650\u3084\u30DB\u30EF\u30A4\u30C8\u30EA\u30B9\u30C8\u3092\u8996\u899A\u7684\u306A\u64CD\u4F5C\u3067\u7C21\u5358\u306B\u8A2D\u5B9A\u3067\u304D\u307E\u3059 ")]),_:1}),e(D,{title:"\u30EF\u30FC\u30EB\u30C9\u3092\u8907\u6570\u4EBA\u3067\u5171\u6709","asset-path":"~assets/Funcs/ShareWorld/Top.png","dialog-component":Tu},{default:t(()=>[s(" \u6700\u65B0\u306E\u30D7\u30EC\u30A4\u30C7\u30FC\u30BF\u3092\u8AB0\u3067\u3082\u8D77\u52D5\u53EF\u80FD\u306B\u3002\u9762\u5012\u306A\u5171\u6709\u4F5C\u696D\u306F\u81EA\u52D5\u5316\u3057\u307E\u3057\u3087\u3046\u3002 ")]),_:1}),e(D,{title:"\u30B5\u30FC\u30D0\u30FC\u306E\u9854\u3092\u30BB\u30C3\u30C8","asset-path":"~assets/Funcs/IconSettings/Top.png","dialog-component":Lu},{default:t(()=>[s(" \u53C2\u52A0\u8005\u304C\u30EF\u30FC\u30EB\u30C9\u306B\u5165\u308B\u969B\u306B\u8868\u793A\u3055\u308C\u308B\u30A2\u30A4\u30B3\u30F3\u3092\uFF0C\u81EA\u7531\u306B\u30AB\u30B9\u30BF\u30DE\u30A4\u30BA\u3067\u304D\u307E\u3059 ")]),_:1})])],64))}});export{u0 as default};
