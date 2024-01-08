import{Q as h}from"./QImg.8d51b05d.js";import{j as P,A as nu,h as i,B as xu,I as yu,J as au,K as wu,L as lu,M as qu,N as ou,r as q,O as Z,P as Iu,c as m,w as O,g as N,R as z,S as ku,U as ru,V as Tu,m as L,W as uu,X as H,E as G,Y as Pu,Z as Lu,i as $u,k as j,$ as su,a0 as Qu,a1 as Vu,a2 as Nu,a3 as Ou,n as iu,o as Hu,q as du,s as cu,t as c,d as e,u as n,a4 as Q,a5 as M,H as S,_ as ju,x as Mu,y as Ku,z as b}from"./index.9191a655.js";import{u as Uu,a as Ru,b as Wu,c as zu,d as Gu,Q as Yu,e as K,f as Ju,g as U}from"./QTabPanels.466631af.js";import{D as R}from"./DownloadBtn.4b15d6ee.js";import{_ as V}from"./SsImg.77de6af1.js";import"./rtl.b51694b1.js";var vu=P({name:"QSlideTransition",props:{appear:Boolean,duration:{type:Number,default:300}},emits:["show","hide"],setup(u,{slots:o,emit:f}){let v=!1,r,C,p=null,t=null,l,D;function E(){r&&r(),r=null,v=!1,p!==null&&(clearTimeout(p),p=null),t!==null&&(clearTimeout(t),t=null),C!==void 0&&C.removeEventListener("transitionend",l),l=null}function _(d,a,F){a!==void 0&&(d.style.height=`${a}px`),d.style.transition=`height ${u.duration}ms cubic-bezier(.25, .8, .50, 1)`,v=!0,r=F}function g(d,a){d.style.overflowY=null,d.style.height=null,d.style.transition=null,E(),a!==D&&f(a)}function I(d,a){let F=0;C=d,v===!0?(E(),F=d.offsetHeight===d.scrollHeight?0:void 0):(D="hide",d.style.overflowY="hidden"),_(d,F,a),p=setTimeout(()=>{p=null,d.style.height=`${d.scrollHeight}px`,l=A=>{t=null,(Object(A)!==A||A.target===d)&&g(d,"show")},d.addEventListener("transitionend",l),t=setTimeout(l,u.duration*1.1)},100)}function k(d,a){let F;C=d,v===!0?E():(D="show",d.style.overflowY="hidden",F=d.scrollHeight),_(d,F,a),p=setTimeout(()=>{p=null,d.style.height=0,l=A=>{t=null,(Object(A)!==A||A.target===d)&&g(d,"hide")},d.addEventListener("transitionend",l),t=setTimeout(l,u.duration*1.1)},100)}return nu(()=>{v===!0&&E()}),()=>i(xu,{css:!1,appear:u.appear,onEnter:I,onLeave:k},o.default)}});const T=yu({}),Xu=Object.keys(au);var eu=P({name:"QExpansionItem",props:{...au,...wu,...lu,icon:String,label:String,labelLines:[Number,String],caption:String,captionLines:[Number,String],dense:Boolean,toggleAriaLabel:String,expandIcon:String,expandedIcon:String,expandIconClass:[Array,String,Object],duration:Number,headerInsetLevel:Number,contentInsetLevel:Number,expandSeparator:Boolean,defaultOpened:Boolean,hideExpandIcon:Boolean,expandIconToggle:Boolean,switchToggleSide:Boolean,denseToggle:Boolean,group:String,popup:Boolean,headerStyle:[Array,String,Object],headerClass:[Array,String,Object]},emits:[...qu,"click","afterShow","afterHide"],setup(u,{slots:o,emit:f}){const{proxy:{$q:v}}=N(),r=ou(u,v),C=q(u.modelValue!==null?u.modelValue:u.defaultOpened),p=q(null),t=Z(),{show:l,hide:D,toggle:E}=Iu({showing:C});let _,g;const I=m(()=>`q-expansion-item q-item-type q-expansion-item--${C.value===!0?"expanded":"collapsed"} q-expansion-item--${u.popup===!0?"popup":"standard"}`),k=m(()=>{if(u.contentInsetLevel===void 0)return null;const s=v.lang.rtl===!0?"Right":"Left";return{["padding"+s]:u.contentInsetLevel*56+"px"}}),d=m(()=>u.disable!==!0&&(u.href!==void 0||u.to!==void 0&&u.to!==null&&u.to!=="")),a=m(()=>{const s={};return Xu.forEach(x=>{s[x]=u[x]}),s}),F=m(()=>d.value===!0||u.expandIconToggle!==!0),A=m(()=>u.expandedIcon!==void 0&&C.value===!0?u.expandedIcon:u.expandIcon||v.iconSet.expansionItem[u.denseToggle===!0?"denseIcon":"icon"]),Bu=m(()=>u.disable!==!0&&(d.value===!0||u.expandIconToggle===!0)),mu=m(()=>({expanded:C.value===!0,detailsId:u.targetUid,toggle:E,show:l,hide:D})),Y=m(()=>{const s=u.toggleAriaLabel!==void 0?u.toggleAriaLabel:v.lang.label[C.value===!0?"collapse":"expand"](u.label);return{role:"button","aria-expanded":C.value===!0?"true":"false","aria-controls":t,"aria-label":s}});O(()=>u.group,s=>{g!==void 0&&g(),s!==void 0&&X()});function fu(s){d.value!==!0&&E(s),f("click",s)}function pu(s){s.keyCode===13&&J(s,!0)}function J(s,x){x!==!0&&p.value!==null&&p.value.focus(),E(s),Pu(s)}function _u(){f("afterShow")}function Eu(){f("afterHide")}function X(){_===void 0&&(_=Z()),C.value===!0&&(T[u.group]=_);const s=O(C,$=>{$===!0?T[u.group]=_:T[u.group]===_&&delete T[u.group]}),x=O(()=>T[u.group],($,bu)=>{bu===_&&$!==void 0&&$!==_&&D()});g=()=>{s(),x(),T[u.group]===_&&delete T[u.group],g=void 0}}function gu(){const s={class:[`q-focusable relative-position cursor-pointer${u.denseToggle===!0&&u.switchToggleSide===!0?" items-end":""}`,u.expandIconClass],side:u.switchToggleSide!==!0,avatar:u.switchToggleSide},x=[i(G,{class:"q-expansion-item__toggle-icon"+(u.expandedIcon===void 0&&C.value===!0?" q-expansion-item__toggle-icon--rotated":""),name:A.value})];return Bu.value===!0&&(Object.assign(s,{tabindex:0,...Y.value,onClick:J,onKeyup:pu}),x.unshift(i("div",{ref:p,class:"q-expansion-item__toggle-focus q-icon q-focus-helper q-focus-helper--rounded",tabindex:-1}))),i(H,s,()=>x)}function Du(){let s;return o.header!==void 0?s=[].concat(o.header(mu.value)):(s=[i(H,()=>[i(uu,{lines:u.labelLines},()=>u.label||""),u.caption?i(uu,{lines:u.captionLines,caption:!0},()=>u.caption):null])],u.icon&&s[u.switchToggleSide===!0?"push":"unshift"](i(H,{side:u.switchToggleSide===!0,avatar:u.switchToggleSide!==!0},()=>i(G,{name:u.icon})))),u.disable!==!0&&u.hideExpandIcon!==!0&&s[u.switchToggleSide===!0?"unshift":"push"](gu()),s}function Au(){const s={ref:"item",style:u.headerStyle,class:u.headerClass,dark:r.value,disable:u.disable,dense:u.dense,insetLevel:u.headerInsetLevel};return F.value===!0&&(s.clickable=!0,s.onClick=fu,Object.assign(s,d.value===!0?a.value:Y.value)),i(ku,s,Du)}function hu(){return ru(i("div",{key:"e-content",class:"q-expansion-item__content relative-position",style:k.value,id:t},L(o.default)),[[Tu,C.value]])}function Su(){const s=[Au(),i(vu,{duration:u.duration,onShow:_u,onHide:Eu},hu)];return u.expandSeparator===!0&&s.push(i(z,{class:"q-expansion-item__border q-expansion-item__border--top absolute-top",dark:r.value}),i(z,{class:"q-expansion-item__border q-expansion-item__border--bottom absolute-bottom",dark:r.value})),s}return u.group!==void 0&&X(),nu(()=>{g!==void 0&&g()}),()=>i("div",{class:I.value},[i("div",{class:"q-expansion-item__container relative-position"},Su())])}}),y=P({name:"QStepperNavigation",setup(u,{slots:o}){return()=>i("div",{class:"q-stepper__nav"},L(o.default))}}),Cu=P({name:"StepHeader",props:{stepper:{},step:{},goToPanel:Function},setup(u,{attrs:o}){const{proxy:{$q:f}}=N(),v=q(null),r=m(()=>u.stepper.modelValue===u.step.name),C=m(()=>{const a=u.step.disable;return a===!0||a===""}),p=m(()=>{const a=u.step.error;return a===!0||a===""}),t=m(()=>{const a=u.step.done;return C.value===!1&&(a===!0||a==="")}),l=m(()=>{const a=u.step.headerNav,F=a===!0||a===""||a===void 0;return C.value===!1&&u.stepper.headerNav&&F}),D=m(()=>u.step.prefix&&(r.value===!1||u.stepper.activeIcon==="none")&&(p.value===!1||u.stepper.errorIcon==="none")&&(t.value===!1||u.stepper.doneIcon==="none")),E=m(()=>{const a=u.step.icon||u.stepper.inactiveIcon;if(r.value===!0){const F=u.step.activeIcon||u.stepper.activeIcon;return F==="none"?a:F||f.iconSet.stepper.active}if(p.value===!0){const F=u.step.errorIcon||u.stepper.errorIcon;return F==="none"?a:F||f.iconSet.stepper.error}if(C.value===!1&&t.value===!0){const F=u.step.doneIcon||u.stepper.doneIcon;return F==="none"?a:F||f.iconSet.stepper.done}return a}),_=m(()=>{const a=p.value===!0?u.step.errorColor||u.stepper.errorColor:void 0;if(r.value===!0){const F=u.step.activeColor||u.stepper.activeColor||u.step.color;return F!==void 0?F:a}return a!==void 0?a:C.value===!1&&t.value===!0?u.step.doneColor||u.stepper.doneColor||u.step.color||u.stepper.inactiveColor:u.step.color||u.stepper.inactiveColor}),g=m(()=>"q-stepper__tab col-grow flex items-center no-wrap relative-position"+(_.value!==void 0?` text-${_.value}`:"")+(p.value===!0?" q-stepper__tab--error q-stepper__tab--error-with-"+(D.value===!0?"prefix":"icon"):"")+(r.value===!0?" q-stepper__tab--active":"")+(t.value===!0?" q-stepper__tab--done":"")+(l.value===!0?" q-stepper__tab--navigation q-focusable q-hoverable":"")+(C.value===!0?" q-stepper__tab--disabled":"")),I=m(()=>u.stepper.headerNav!==!0?!1:l.value);function k(){v.value!==null&&v.value.focus(),r.value===!1&&u.goToPanel(u.step.name)}function d(a){a.keyCode===13&&r.value===!1&&u.goToPanel(u.step.name)}return()=>{const a={class:g.value};l.value===!0&&(a.onClick=k,a.onKeyup=d,Object.assign(a,C.value===!0?{tabindex:-1,"aria-disabled":"true"}:{tabindex:o.tabindex||0}));const F=[i("div",{class:"q-focus-helper",tabindex:-1,ref:v}),i("div",{class:"q-stepper__dot row flex-center q-stepper__line relative-position"},[i("span",{class:"row flex-center"},[D.value===!0?u.step.prefix:i(G,{name:E.value})])])];if(u.step.title!==void 0&&u.step.title!==null){const A=[i("div",{class:"q-stepper__title"},u.step.title)];u.step.caption!==void 0&&u.step.caption!==null&&A.push(i("div",{class:"q-stepper__caption"},u.step.caption)),F.push(i("div",{class:"q-stepper__label q-stepper__line relative-position"},A))}return ru(i("div",a,F),[[Lu,I.value]])}}});function Fu(u){return i("div",{class:"q-stepper__step-content"},[i("div",{class:"q-stepper__step-inner"},L(u.default))])}const tu={setup(u,{slots:o}){return()=>Fu(o)}};var w=P({name:"QStep",props:{...Uu,icon:String,color:String,title:{type:String,required:!0},caption:String,prefix:[String,Number],doneIcon:String,doneColor:String,activeIcon:String,activeColor:String,errorIcon:String,errorColor:String,headerNav:{type:Boolean,default:!0},done:Boolean,error:Boolean,onScroll:[Function,Array]},setup(u,{slots:o,emit:f}){const{proxy:{$q:v}}=N(),r=$u(su,j);if(r===j)return console.error("QStep needs to be a child of QStepper"),j;const{getCacheWithFn:C}=Ru(),p=q(null),t=m(()=>r.value.modelValue===u.name),l=m(()=>v.platform.is.ios!==!0&&v.platform.is.chrome===!0||t.value!==!0||r.value.vertical!==!0?{}:{onScroll(_){const{target:g}=_;g.scrollTop>0&&(g.scrollTop=0),u.onScroll!==void 0&&f("scroll",_)}}),D=m(()=>typeof u.name=="string"||typeof u.name=="number"?u.name:String(u.name));function E(){const _=r.value.vertical;return _===!0&&r.value.keepAlive===!0?i(Qu,r.value.keepAliveProps.value,t.value===!0?[i(r.value.needsUniqueKeepAliveWrapper.value===!0?C(D.value,()=>({...tu,name:D.value})):tu,{key:D.value},o.default)]:void 0):_!==!0||t.value===!0?Fu(o):void 0}return()=>i("div",{ref:p,class:"q-stepper__step",role:"tabpanel",...l.value},r.value.vertical===!0?[i(Cu,{stepper:r.value,step:u,goToPanel:r.value.goToPanel}),r.value.animated===!0?i(vu,E):E()]:[E()])}});const Zu=/(-\w)/g;function ue(u){const o={};for(const f in u){const v=f.replace(Zu,r=>r[1].toUpperCase());o[v]=u[f]}return o}var W=P({name:"QStepper",props:{...lu,...Wu,flat:Boolean,bordered:Boolean,alternativeLabels:Boolean,headerNav:Boolean,contracted:Boolean,headerClass:String,inactiveColor:String,inactiveIcon:String,doneIcon:String,doneColor:String,activeIcon:String,activeColor:String,errorIcon:String,errorColor:String},emits:zu,setup(u,{slots:o}){const f=N(),v=ou(u,f.proxy.$q),{updatePanelsList:r,isValidPanelName:C,updatePanelIndex:p,getPanelContent:t,getPanels:l,panelDirectives:D,goToPanel:E,keepAliveProps:_,needsUniqueKeepAliveWrapper:g}=Gu();Vu(su,m(()=>({goToPanel:E,keepAliveProps:_,needsUniqueKeepAliveWrapper:g,...u})));const I=m(()=>`q-stepper q-stepper--${u.vertical===!0?"vertical":"horizontal"}`+(u.flat===!0?" q-stepper--flat":"")+(u.bordered===!0?" q-stepper--bordered":"")+(v.value===!0?" q-stepper--dark q-dark":"")),k=m(()=>`q-stepper__header row items-stretch justify-between q-stepper__header--${u.alternativeLabels===!0?"alternative":"standard"}-labels`+(u.flat===!1||u.bordered===!0?" q-stepper__header--border":"")+(u.contracted===!0?" q-stepper__header--contracted":"")+(u.headerClass!==void 0?` ${u.headerClass}`:""));function d(){const a=L(o.message,[]);if(u.vertical===!0){C(u.modelValue)&&p();const F=i("div",{class:"q-stepper__content"},L(o.default));return a===void 0?[F]:a.concat(F)}return[i("div",{class:k.value},l().map(F=>{const A=ue(F.props);return i(Cu,{key:A.name,stepper:u,step:A,goToPanel:E})})),a,Ou("div",{class:"q-stepper__content q-panel-parent"},t(),"cont",u.swipeable,()=>D.value)]}return()=>(r(o),i("div",{class:I.value},Nu(o.navigation,d())))}}),ee="/ServerStarter/assets/Edge_Save1.1d8bcc69.png",te="/ServerStarter/assets/Edge_Save2.07ab148d.png",ne="/ServerStarter/assets/Icon.765867c5.png",ae="/ServerStarter/assets/WelcomeWindow.91e5d79d.png",le="/ServerStarter/assets/OwnerPlayerSetting1.8ddd3e20.png",oe="/ServerStarter/assets/OwnerPlayerSetting2.8e07dc7c.png",re="/ServerStarter/assets/MainWindow.8c3718c0.png",se="/ServerStarter/assets/Server2.e0ff78b2.png",ie="/ServerStarter/assets/Launcher.b32956d7.png",de="/ServerStarter/assets/client.927a1f28.png",ce="/ServerStarter/assets/ip.c4c1fadc.png",ve="/ServerStarter/assets/client2.8fa4ad48.png";const B=u=>(Mu("data-v-7b89673c"),u=u(),Ku(),u),Ce=B(()=>n("h1",null,"ServerStarter\u3078\u3088\u3046\u3053\u305D\uFF01",-1)),Fe=B(()=>n("p",null,"ServerStarter\u306FMinecraft\u306E\u30DE\u30EB\u30C1\u30B5\u30FC\u30D0\u30FC\u3092\u30DC\u30BF\u30F3\u30AF\u30EA\u30C3\u30AF\u306B\u3088\u3063\u3066\u7C21\u5358\u306B\u7ACB\u3066\u3089\u308C\u308B\u3088\u3046\u306B\u3059\u308B\u30BD\u30D5\u30C8\u30A6\u30A7\u30A2\u3067\u3059",-1)),Be=B(()=>n("p",null,"\u30EF\u30F3\u30AF\u30EA\u30C3\u30AF\u3067\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\uFF0C\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u306E\u4E16\u754C\u306B\u30C0\u30A4\u30D6\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),me=B(()=>n("h1",null,"\u5C0E\u5165\u65B9\u6CD5",-1)),fe=B(()=>n("p",null,[b(" ServerStarter2\u306E\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u3092\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u3057\u307E\u3057\u3087\u3046"),n("br"),b(" \u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u3092\u8D77\u52D5\u3057\u3066ServerStarter\u3092PC\u306B\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB\u3057\u3066\u304F\u3060\u3055\u3044 ")],-1)),pe={class:"q-py-md q-gutter-md"},_e=B(()=>n("p",null,[b(" ServerStarter\u306F\u500B\u4EBA\u958B\u767A\u306E\u305F\u3081\uFF0C\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6570\u304C\u5341\u5206\u306A\u6570\u306B\u306A\u308B\u307E\u3067\u76F8\u5F53\u306E\u6642\u9593\u304C\u304B\u304B\u308A\u307E\u3059"),n("br"),b(" \u4E00\u90E8\u306E\u30D6\u30E9\u30A6\u30B6\u3067\u306F\u5341\u5206\u306A\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6570\u306E\u306A\u3044\u30BD\u30D5\u30C8\u306B\u5BFE\u3057\u3066\u8B66\u544A\u3092\u51FA\u3059\u305F\u3081\uFF0C\u30BD\u30D5\u30C8\u3092\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u3067\u304D\u306A\u3044\u3053\u3068\u304C\u3042\u308A\u307E\u3059 ")],-1)),Ee=B(()=>n("p",null,"Edge\u3067\u306F\u753B\u50CF\u306E\u3088\u3046\u306B\u53F3\u5074\u306E\u300C\u30FB\u30FB\u30FB\u300D\u304B\u3089\u4FDD\u5B58\u3092\u62BC\u3057\uFF0C\u6B21\u306E\u753B\u9762\u3067\u300C\u4FDD\u6301\u3059\u308B\u300D\u3092\u30AF\u30EA\u30C3\u30AF\u3059\u308B\u3053\u3068\u3067\uFF0C\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u306E\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u304C\u59CB\u307E\u308A\u307E\u3059",-1)),ge=B(()=>n("p",null,[b(" \u306A\u304A\uFF0CServerStarter\u306E\u30BD\u30FC\u30B9\u30B3\u30FC\u30C9\u306F "),n("a",{href:"https://github.com/CivilTT/ServerStarter",target:"_blank",class:"a"},"GitHub"),b(" \u306B\u3066\u516C\u958B\u3057\u3066\u304A\u308A\u307E\u3059\u306E\u3067\uFF0C\u305D\u3061\u3089\u3082\u3054\u78BA\u8A8D\u3044\u305F\u3060\u3051\u307E\u3059\u3068\u5E78\u3044\u3067\u3059 ")],-1)),De={class:"row q-gutter-md items-center"},Ae=B(()=>n("p",null,[b(" ServerStarter2\u3067\u306F\u73FE\u5728\uFF0C\u30A2\u30D7\u30EA\u306B\u5BFE\u3059\u308B\u7F72\u540D\u3092\u4ED8\u3051\u308B\u3053\u3068\u304C\u3067\u304D\u3066\u3044\u307E\u305B\u3093"),n("br"),b(" \u3053\u306E\u305F\u3081\uFF0C\u4EE5\u4E0B\u306E\u3088\u3046\u306A\u8B66\u544A\u304C\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB\u6642\u306B\u8868\u793A\u3055\u308C\u308B\u3053\u3068\u304C\u3042\u308A\u307E\u3059\u306E\u3067\uFF0C\u4E0B\u8A18\u306E\u624B\u9806\u306B\u5F93\u3063\u3066\u64CD\u4F5C\u3092\u304A\u9858\u3044\u3057\u307E\u3059 ")],-1)),he=B(()=>n("li",null,"\u8868\u793A\u3055\u308C\u305F\u753B\u9762\u4E2D\u90E8\u306B\u3042\u308B\u300C\u8A73\u7D30\u8A2D\u5B9A\u300D\u3092\u30AF\u30EA\u30C3\u30AF",-1)),Se=B(()=>n("li",null,"\u300C\u5B9F\u884C\u300D\u3092\u30AF\u30EA\u30C3\u30AF\u3059\u308B\u3068\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB\u304C\u958B\u59CB\u3055\u308C\u307E\u3059",-1)),be=B(()=>n("li",null,"\u3053\u306E\u753B\u9762\u306FOK\u3092\u62BC\u3057\u3066\u9589\u3058\u308B",-1)),xe=B(()=>n("li",null,"\u300C\u30B7\u30B9\u30C6\u30E0\u74B0\u5883\u8A2D\u5B9A\u300D\uFF1E\u300C\u30BB\u30AD\u30E5\u30EA\u30C6\u30A3\u3068\u30D7\u30E9\u30A4\u30D0\u30B7\u30FC\u300D\uFF1E\u300C\u4E00\u822C\u300D\u306E\u9806\u306B\u958B\u304D\uFF0C\u300C\u3053\u306E\u307E\u307E\u958B\u304F\u300D\u3092\u62BC\u3059\u3068\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB\u304C\u958B\u59CB\u3055\u308C\u307E\u3059",-1)),ye={class:"row q-gutter-md"},we=B(()=>n("p",null,"\u30C7\u30B9\u30AF\u30C8\u30C3\u30D7\u306B\u30A2\u30A4\u30B3\u30F3\u304C\u4F5C\u6210\u3055\u308C\u308B\u305F\u3081\uFF0C\u3053\u308C\u3092\u30C0\u30D6\u30EB\u30AF\u30EA\u30C3\u30AF\u3057\u3066\u8D77\u52D5",-1)),qe=B(()=>n("p",null,"\u8A00\u8A9E\u8A2D\u5B9A\u3092\u78BA\u8A8D\u3057\u3001\u5229\u7528\u898F\u7D04\u306B\u540C\u610F\u3059\u308C\u3070\u300C\u30B9\u30BF\u30FC\u30C8\u300D\uFF01",-1)),Ie=B(()=>n("p",null,"Minecraft\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u30A2\u30AB\u30A6\u30F3\u30C8\u3092\u6301\u3063\u3066\u3044\u308B\u5834\u5408\u306F\u3001\u30B2\u30FC\u30E0\u5185\u3067\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u540D\u3092\u767B\u9332\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),ke=B(()=>n("p",null,[b(" \u81EA\u8EAB\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u540D\u3092\u5165\u529B\u3059\u308B\u3068\u3001\u753B\u50CF\u306E\u3088\u3046\u306B\u5019\u88DC\u304C\u8868\u793A\u3055\u308C\u307E\u3059"),n("br"),b(" \u300C\u3053\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u3092\u767B\u9332\u300D\u30DC\u30BF\u30F3\u3092\u62BC\u3057\u3001\u300C\u30AA\u30FC\u30CA\u30FC\u3092\u767B\u9332\u300D\u3092\u30AF\u30EA\u30C3\u30AF\u3057\u307E\u3057\u3087\u3046\uFF01 ")],-1)),Te={class:"row q-gutter-md items-center"},Pe=B(()=>n("h1",null,"\u30B5\u30FC\u30D0\u30FC\u306E\u8D77\u52D5",-1)),Le=B(()=>n("p",null,"\u30EF\u30FC\u30EB\u30C9\u540D\u3092\u597D\u304D\u306A\u540D\u79F0\u306B\u5909\u66F4\u3057\u3001\u30B5\u30FC\u30D0\u30FC\u306E\u30D0\u30FC\u30B8\u30E7\u30F3\u6307\u5B9A\u3001\u306A\u3069\u304C\u3067\u304D\u307E\u3059",-1)),$e=B(()=>n("p",{class:"text-red text-bold"},"\u300C\u30DD\u30FC\u30C8\u958B\u653E\u4E0D\u8981\u5316\u300D\u3067\u306F\u3054\u53CB\u4EBA\u306A\u3069\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u5165\u5BA4\u3059\u308B\u305F\u3081\u306E\u8A2D\u5B9A\u3092\u884C\u3044\u307E\u3059\u306E\u3067\u304A\u5FD8\u308C\u306A\u304F\uFF01",-1)),Qe=B(()=>n("p",null,"\u30D7\u30ED\u30D1\u30C6\u30A3\u30BF\u30B0\u3084\u30D7\u30EC\u30A4\u30E4\u30FC\u30BF\u30B0\u3067\u3082\u8A73\u7D30\u306A\u8A2D\u5B9A\u304C\u3067\u304D\u307E\u3059\uFF01",-1)),Ve=B(()=>n("p",null,"\u6E96\u5099\u304C\u6574\u3063\u305F\u3089\u3001\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),Ne=B(()=>n("p",null,[b(" \u30B5\u30FC\u30D0\u30FC\u304C\u8D77\u52D5\u3059\u308B\u3068\u3053\u306E\u3088\u3046\u306A\u753B\u9762\u304C\u8868\u793A\u3055\u308C\u307E\u3059\uFF01"),n("br"),b(" \u904A\u3073\u7D42\u308F\u3063\u305F\u5F8C\u306B\u306F\u5DE6\u4E0B\u306E\u300C\u505C\u6B62\u300D\u30DC\u30BF\u30F3\u3092\u62BC\u3057\u3066\u30B5\u30FC\u30D0\u30FC\u3092\u9589\u3058\u307E\u3057\u3087\u3046\uFF01 ")],-1)),Oe=B(()=>n("p",null," \u203B \u521D\u56DE\u8D77\u52D5\u6642\u306F\u8D77\u52D5\u524D\u306BEula\uFF08\u5229\u7528\u898F\u7D04\uFF09\u3078\u306E\u540C\u610F\u3092\u6C42\u3081\u3089\u308C\u307E\u3059 ",-1)),He=B(()=>n("h1",null,"\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0",-1)),je=B(()=>n("p",null," Minecraft Launcher\u3092\u8D77\u52D5\u3057\uFF0C\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\u305F\u30D0\u30FC\u30B8\u30E7\u30F3\u3067\u30B2\u30FC\u30E0\u3092\u958B\u59CB\u3057\u3066\u304F\u3060\u3055\u3044 ",-1)),Me=B(()=>n("p",null," \u30D0\u30FC\u30B8\u30E7\u30F3\u304C\u306A\u3044\u6642\u306B\u306F\u300C\u8D77\u52D5\u69CB\u6210\u300D\u304B\u3089\u8D77\u52D5\u3057\u305F\u3044\u30D0\u30FC\u30B8\u30E7\u30F3\u306E\u8FFD\u52A0\u304C\u5FC5\u8981\u3067\u3059\uFF01 ",-1)),Ke=B(()=>n("p",null," Minecraft\u304C\u8D77\u52D5\u3057\u305F\u5F8C\u306F\u4EE5\u4E0B\u306E\u624B\u9806\u3067\u3054\u81EA\u8EAB\u306E\u30B5\u30FC\u30D0\u30FC\u3092\u8FFD\u52A0\u3057\u307E\u3057\u3087\u3046\uFF01 ",-1)),Ue=B(()=>n("ol",null,[n("li",null,"\u753B\u9762\u4E2D\u592E\u306E\u300C\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u300D\u3092\u9078\u629E"),n("li",null,"\u753B\u9762\u53F3\u4E0B\u306E\u300C\u30B5\u30FC\u30D0\u30FC\u3092\u8FFD\u52A0\u300D\u3092\u9078\u629E"),n("li",null,"\u958B\u3044\u305F\u753B\u9762\u306B\u5BFE\u3057\u3066\u753B\u50CF\u306E\u3088\u3046\u306B\u5404\u9805\u76EE\u3092\u8A2D\u5B9A")],-1)),Re=B(()=>n("p",null," \u300C\u5B8C\u4E86\u300D\u3092\u62BC\u3057\u3066\u4F5C\u6210\u3055\u308C\u305F\u30B5\u30FC\u30D0\u30FC\u306B\u5165\u308C\u3070\u6E96\u5099\u5B8C\u4E86\u3067\u3059\uFF01 ",-1)),We=B(()=>n("p",null," \u3054\u53CB\u4EBA\u306A\u3069\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0\u3059\u308B\u969B\u306B\u306F\u3001 \u30B5\u30FC\u30D0\u30FC\u8D77\u52D5\u6642\u306BServerStarter\u306E\u753B\u9762\u53F3\u4E0A\u306B\u6620\u3063\u3066\u3044\u308BIP\u30A2\u30C9\u30EC\u30B9\u3092\u3001Minecraft\u306E\u30B5\u30FC\u30D0\u30FC\u30A2\u30C9\u30EC\u30B9\u6B04\u306B\u5165\u529B\u3057\u3066\u304F\u3060\u3055\u3044 ",-1)),ze={class:"row q-gutter-md"},Ge=iu({__name:"HowToStart",setup(u){const o=q(1),f=q(1),v=q(1),r=q("win"),C=q("");return Hu(async()=>{const p=await fetch("https://api.github.com/repos/CivilTT/ServerStarter2/releases/latest");C.value=(await p.json()).name}),(p,t)=>(du(),cu(Q,{flat:"",style:{"max-width":"100%"}},{default:c(()=>[e(M,null,{default:c(()=>[Ce,Fe,Be,me,e(W,{modelValue:o.value,"onUpdate:modelValue":t[8]||(t[8]=l=>o.value=l),flat:"",animated:""},{default:c(()=>[e(w,{name:1,title:"1. \u5C0E\u5165",done:o.value>1},{default:c(()=>[fe,n("div",pe,[e(eu,{label:"\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6642\u306B\u8B66\u544A\u304C\u51FA\u305F\u5834\u5408","header-class":"bg-orange-4"},{default:c(()=>[e(Q,{flat:"",class:"bg-orange-2"},{default:c(()=>[e(M,null,{default:c(()=>[_e,Ee,ge,n("div",De,[e(h,{src:ee,style:{"min-width":"15rem"},class:"col fit"}),e(h,{src:te,style:{"min-width":"15rem"},class:"col"})])]),_:1})]),_:1})]),_:1}),e(eu,{label:"\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB\u6642\u306B\u8B66\u544A\u304C\u51FA\u305F\u5834\u5408","header-class":"bg-orange-4"},{default:c(()=>[e(Q,{flat:"",class:"bg-orange-2"},{default:c(()=>[e(M,null,{default:c(()=>[Ae,e(Q,{class:"fit"},{default:c(()=>[e(Yu,{modelValue:r.value,"onUpdate:modelValue":t[0]||(t[0]=l=>r.value=l),dense:"","active-color":"primary","indicator-color":"primary",align:"justify"},{default:c(()=>[e(K,{"no-caps":"",name:"win",label:"Windows"}),e(K,{"no-caps":"",name:"mac",label:"Mac OS"}),e(K,{disable:"","no-caps":"",name:"linux",label:"Linux"})]),_:1},8,["modelValue"]),e(z),e(Ju,{modelValue:r.value,"onUpdate:modelValue":t[1]||(t[1]=l=>r.value=l),animated:""},{default:c(()=>[e(U,{name:"win"},{default:c(()=>[n("ol",null,[he,e(V,{path:"assets/Introduction/defender1.png",style:{"max-width":"15rem"}}),Se,e(V,{path:"assets/Introduction/defender2.png",style:{"max-width":"15rem"}})])]),_:1}),e(U,{name:"mac"},{default:c(()=>[n("ol",null,[be,e(V,{path:"assets/Introduction/unopen.png",style:{"max-width":"15rem"}}),xe,e(V,{path:"assets/Introduction/privacy.png",style:{"max-width":"15rem"}})])]),_:1}),e(U,{name:"linux"})]),_:1},8,["modelValue"])]),_:1})]),_:1})]),_:1})]),_:1})]),n("div",ye,[e(R,{version:C.value,outline:"","os-name":"windows"},null,8,["version"]),e(R,{version:C.value,outline:"","os-name":"mac"},null,8,["version"]),e(R,{version:C.value,outline:"",disable:"","os-name":"linux"},null,8,["version"])]),e(y,null,{default:c(()=>[e(S,{onClick:t[2]||(t[2]=l=>o.value=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),e(w,{name:2,title:"2. \u8D77\u52D5",done:o.value>2},{default:c(()=>[we,e(h,{src:ne,width:"min(200px,100%)"}),e(y,null,{default:c(()=>[e(S,{onClick:t[3]||(t[3]=l=>o.value=3),color:"secondary",label:"next"}),e(S,{flat:"",onClick:t[4]||(t[4]=l=>o.value=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),e(w,{name:3,title:"3-1. \u521D\u671F\u8A2D\u5B9A\uFF08\u5229\u7528\u898F\u7D04\uFF09",done:o.value>3},{default:c(()=>[qe,e(h,{src:ae,width:"min(500px,100%)"}),e(y,null,{default:c(()=>[e(S,{onClick:t[5]||(t[5]=l=>o.value=4),color:"secondary",label:"next"}),e(S,{flat:"",onClick:t[6]||(t[6]=l=>o.value=2),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),e(w,{name:4,title:"3-2. \u521D\u671F\u8A2D\u5B9A\uFF08\u30D7\u30EC\u30A4\u30E4\u30FC\u767B\u9332\uFF09",done:o.value>3},{default:c(()=>[Ie,ke,n("div",Te,[e(h,{src:le,style:{"min-width":"15rem"},class:"col fit"}),e(h,{src:oe,style:{"min-width":"15rem"},class:"col"})]),e(y,null,{default:c(()=>[e(S,{flat:"",onClick:t[7]||(t[7]=l=>o.value=3),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"]),Pe,e(W,{modelValue:f.value,"onUpdate:modelValue":t[11]||(t[11]=l=>f.value=l),flat:"",animated:""},{default:c(()=>[e(w,{name:1,title:"1. \u8A2D\u5B9A",done:f.value>1},{default:c(()=>[Le,$e,Qe,e(h,{src:re,width:"min(900px,100%)"}),e(y,null,{default:c(()=>[e(S,{onClick:t[9]||(t[9]=l=>f.value=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),e(w,{name:2,title:"2. \u8D77\u52D5",done:f.value>2},{default:c(()=>[Ve,Ne,Oe,e(h,{src:se,width:"min(700px,100%)"}),e(y,null,{default:c(()=>[e(S,{flat:"",onClick:t[10]||(t[10]=l=>f.value=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"]),He,e(W,{modelValue:v.value,"onUpdate:modelValue":t[16]||(t[16]=l=>v.value=l),flat:"",animated:""},{default:c(()=>[e(w,{name:1,title:"1. Minecraft\u306E\u8D77\u52D5",done:v.value>1},{default:c(()=>[je,Me,e(h,{src:ie,width:"min(500px,100%)"}),e(y,null,{default:c(()=>[e(S,{onClick:t[12]||(t[12]=l=>v.value=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),e(w,{name:2,title:"2. \u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u306E\u8A2D\u5B9A",done:v.value>2},{default:c(()=>[Ke,Ue,Re,e(h,{src:de,width:"min(500px,100%)"}),e(y,null,{default:c(()=>[e(S,{onClick:t[13]||(t[13]=l=>v.value=3),color:"secondary",label:"next"}),e(S,{flat:"",onClick:t[14]||(t[14]=l=>v.value=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),e(w,{name:3,title:"3. \u3054\u53CB\u4EBA\u306A\u3069\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0\u3059\u308B",done:v.value>3},{default:c(()=>[We,n("div",ze,[e(h,{src:ce,style:{"min-width":"15rem"},class:"col"}),e(h,{src:ve,style:{"min-width":"15rem"},class:"col"})]),e(y,null,{default:c(()=>[e(S,{flat:"",onClick:t[15]||(t[15]=l=>v.value=2),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"])]),_:1})]),_:1}))}});var Ye=ju(Ge,[["__scopeId","data-v-7b89673c"]]);const n0=iu({__name:"IntroductionView",setup(u){return(o,f)=>(du(),cu(Ye))}});export{n0 as default};