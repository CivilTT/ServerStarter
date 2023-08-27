import{Q as I}from"./QImg.01b01ef2.js";import{j,D as he,h as c,E as De,H as $e,I as Ee,J as Ne,K as _e,L as Ve,M as Ae,r as N,N as ce,O as Qe,c as g,w as ee,g as U,P as de,R as Me,S as be,U as He,q as M,V as ve,W as ae,Q as se,X as ue,Y as Oe,Z as je,$ as W,a0 as pe,a1 as Ke,a2 as Ce,a3 as Re,a4 as Z,a5 as oe,a6 as fe,a7 as We,a8 as le,a9 as Ue,aa as ze,ab as Se,i as Ye,m as re,ac as xe,ad as Ge,ae as Xe,af as Je,s as ye,ag as Ze,t as Ie,v as qe,x as h,d as l,ah as w,b as x,y as m,ai as Fe,aj as me,z as q,C as k,_ as eu,A as uu,B as tu}from"./index.be41a949.js";var we=j({name:"QSlideTransition",props:{appear:Boolean,duration:{type:Number,default:300}},emits:["show","hide"],setup(e,{slots:t,emit:s}){let o=!1,r,u,a=null,n=null,D,_;function f(){r&&r(),r=null,o=!1,a!==null&&(clearTimeout(a),a=null),n!==null&&(clearTimeout(n),n=null),u!==void 0&&u.removeEventListener("transitionend",D),D=null}function B(d,i,C){i!==void 0&&(d.style.height=`${i}px`),d.style.transition=`height ${e.duration}ms cubic-bezier(.25, .8, .50, 1)`,o=!0,r=C}function F(d,i){d.style.overflowY=null,d.style.height=null,d.style.transition=null,f(),i!==_&&s(i)}function S(d,i){let C=0;u=d,o===!0?(f(),C=d.offsetHeight===d.scrollHeight?0:void 0):(_="hide",d.style.overflowY="hidden"),B(d,C,i),a=setTimeout(()=>{a=null,d.style.height=`${d.scrollHeight}px`,D=b=>{n=null,(Object(b)!==b||b.target===d)&&F(d,"show")},d.addEventListener("transitionend",D),n=setTimeout(D,e.duration*1.1)},100)}function y(d,i){let C;u=d,o===!0?f():(_="show",d.style.overflowY="hidden",C=d.scrollHeight),B(d,C,i),a=setTimeout(()=>{a=null,d.style.height=0,D=b=>{n=null,(Object(b)!==b||b.target===d)&&F(d,"hide")},d.addEventListener("transitionend",D),n=setTimeout(D,e.duration*1.1)},100)}return he(()=>{o===!0&&f()}),()=>c(De,{css:!1,appear:e.appear,onEnter:S,onLeave:y},t.default)}});const Q=$e({}),nu=Object.keys(Ee);var au=j({name:"QExpansionItem",props:{...Ee,...Ne,..._e,icon:String,label:String,labelLines:[Number,String],caption:String,captionLines:[Number,String],dense:Boolean,toggleAriaLabel:String,expandIcon:String,expandedIcon:String,expandIconClass:[Array,String,Object],duration:Number,headerInsetLevel:Number,contentInsetLevel:Number,expandSeparator:Boolean,defaultOpened:Boolean,hideExpandIcon:Boolean,expandIconToggle:Boolean,switchToggleSide:Boolean,denseToggle:Boolean,group:String,popup:Boolean,headerStyle:[Array,String,Object],headerClass:[Array,String,Object]},emits:[...Ve,"click","afterShow","afterHide"],setup(e,{slots:t,emit:s}){const{proxy:{$q:o}}=U(),r=Ae(e,o),u=N(e.modelValue!==null?e.modelValue:e.defaultOpened),a=N(null),n=ce(),{show:D,hide:_,toggle:f}=Qe({showing:u});let B,F;const S=g(()=>`q-expansion-item q-item-type q-expansion-item--${u.value===!0?"expanded":"collapsed"} q-expansion-item--${e.popup===!0?"popup":"standard"}`),y=g(()=>{if(e.contentInsetLevel===void 0)return null;const p=o.lang.rtl===!0?"Right":"Left";return{["padding"+p]:e.contentInsetLevel*56+"px"}}),d=g(()=>e.disable!==!0&&(e.href!==void 0||e.to!==void 0&&e.to!==null&&e.to!=="")),i=g(()=>{const p={};return nu.forEach(T=>{p[T]=e[T]}),p}),C=g(()=>d.value===!0||e.expandIconToggle!==!0),b=g(()=>e.expandedIcon!==void 0&&u.value===!0?e.expandedIcon:e.expandIcon||o.iconSet.expansionItem[e.denseToggle===!0?"denseIcon":"icon"]),H=g(()=>e.disable!==!0&&(d.value===!0||e.expandIconToggle===!0)),K=g(()=>({expanded:u.value===!0,detailsId:e.targetUid,toggle:f,show:D,hide:_})),z=g(()=>{const p=e.toggleAriaLabel!==void 0?e.toggleAriaLabel:o.lang.label[u.value===!0?"collapse":"expand"](e.label);return{role:"button","aria-expanded":u.value===!0?"true":"false","aria-controls":n,"aria-label":p}});ee(()=>e.group,p=>{F!==void 0&&F(),p!==void 0&&X()});function Y(p){d.value!==!0&&f(p),s("click",p)}function V(p){p.keyCode===13&&R(p,!0)}function R(p,T){T!==!0&&a.value!==null&&a.value.focus(),f(p),ue(p)}function G(){s("afterShow")}function te(){s("afterHide")}function X(){B===void 0&&(B=ce()),u.value===!0&&(Q[e.group]=B);const p=ee(u,J=>{J===!0?Q[e.group]=B:Q[e.group]===B&&delete Q[e.group]}),T=ee(()=>Q[e.group],(J,Le)=>{Le===B&&J!==void 0&&J!==B&&_()});F=()=>{p(),T(),Q[e.group]===B&&delete Q[e.group],F=void 0}}function ne(){const p={class:[`q-focusable relative-position cursor-pointer${e.denseToggle===!0&&e.switchToggleSide===!0?" items-end":""}`,e.expandIconClass],side:e.switchToggleSide!==!0,avatar:e.switchToggleSide},T=[c(se,{class:"q-expansion-item__toggle-icon"+(e.expandedIcon===void 0&&u.value===!0?" q-expansion-item__toggle-icon--rotated":""),name:b.value})];return H.value===!0&&(Object.assign(p,{tabindex:0,...z.value,onClick:R,onKeyup:V}),T.unshift(c("div",{ref:a,class:"q-expansion-item__toggle-focus q-icon q-focus-helper q-focus-helper--rounded",tabindex:-1}))),c(ae,p,()=>T)}function v(){let p;return t.header!==void 0?p=[].concat(t.header(K.value)):(p=[c(ae,()=>[c(ve,{lines:e.labelLines},()=>e.label||""),e.caption?c(ve,{lines:e.captionLines,caption:!0},()=>e.caption):null])],e.icon&&p[e.switchToggleSide===!0?"push":"unshift"](c(ae,{side:e.switchToggleSide===!0,avatar:e.switchToggleSide!==!0},()=>c(se,{name:e.icon})))),e.disable!==!0&&e.hideExpandIcon!==!0&&p[e.switchToggleSide===!0?"unshift":"push"](ne()),p}function A(){const p={ref:"item",style:e.headerStyle,class:e.headerClass,dark:r.value,disable:e.disable,dense:e.dense,insetLevel:e.headerInsetLevel};return C.value===!0&&(p.clickable=!0,p.onClick=Y,Object.assign(p,d.value===!0?i.value:z.value)),c(Me,p,v)}function P(){return be(c("div",{key:"e-content",class:"q-expansion-item__content relative-position",style:y.value,id:n},M(t.default)),[[He,u.value]])}function O(){const p=[A(),c(we,{duration:e.duration,onShow:G,onHide:te},P)];return e.expandSeparator===!0&&p.push(c(de,{class:"q-expansion-item__border q-expansion-item__border--top absolute-top",dark:r.value}),c(de,{class:"q-expansion-item__border q-expansion-item__border--bottom absolute-bottom",dark:r.value})),p}return e.group!==void 0&&X(),he(()=>{F!==void 0&&F()}),()=>c("div",{class:S.value},[c("div",{class:"q-expansion-item__container relative-position"},O())])}}),L=j({name:"QStepperNavigation",setup(e,{slots:t}){return()=>c("div",{class:"q-stepper__nav"},M(t.default))}}),ke=j({name:"StepHeader",props:{stepper:{},step:{},goToPanel:Function},setup(e,{attrs:t}){const{proxy:{$q:s}}=U(),o=N(null),r=g(()=>e.stepper.modelValue===e.step.name),u=g(()=>{const i=e.step.disable;return i===!0||i===""}),a=g(()=>{const i=e.step.error;return i===!0||i===""}),n=g(()=>{const i=e.step.done;return u.value===!1&&(i===!0||i==="")}),D=g(()=>{const i=e.step.headerNav,C=i===!0||i===""||i===void 0;return u.value===!1&&e.stepper.headerNav&&C}),_=g(()=>e.step.prefix&&(r.value===!1||e.stepper.activeIcon==="none")&&(a.value===!1||e.stepper.errorIcon==="none")&&(n.value===!1||e.stepper.doneIcon==="none")),f=g(()=>{const i=e.step.icon||e.stepper.inactiveIcon;if(r.value===!0){const C=e.step.activeIcon||e.stepper.activeIcon;return C==="none"?i:C||s.iconSet.stepper.active}if(a.value===!0){const C=e.step.errorIcon||e.stepper.errorIcon;return C==="none"?i:C||s.iconSet.stepper.error}if(u.value===!1&&n.value===!0){const C=e.step.doneIcon||e.stepper.doneIcon;return C==="none"?i:C||s.iconSet.stepper.done}return i}),B=g(()=>{const i=a.value===!0?e.step.errorColor||e.stepper.errorColor:void 0;if(r.value===!0){const C=e.step.activeColor||e.stepper.activeColor||e.step.color;return C!==void 0?C:i}return i!==void 0?i:u.value===!1&&n.value===!0?e.step.doneColor||e.stepper.doneColor||e.step.color||e.stepper.inactiveColor:e.step.color||e.stepper.inactiveColor}),F=g(()=>"q-stepper__tab col-grow flex items-center no-wrap relative-position"+(B.value!==void 0?` text-${B.value}`:"")+(a.value===!0?" q-stepper__tab--error q-stepper__tab--error-with-"+(_.value===!0?"prefix":"icon"):"")+(r.value===!0?" q-stepper__tab--active":"")+(n.value===!0?" q-stepper__tab--done":"")+(D.value===!0?" q-stepper__tab--navigation q-focusable q-hoverable":"")+(u.value===!0?" q-stepper__tab--disabled":"")),S=g(()=>e.stepper.headerNav!==!0?!1:D.value);function y(){o.value!==null&&o.value.focus(),r.value===!1&&e.goToPanel(e.step.name)}function d(i){i.keyCode===13&&r.value===!1&&e.goToPanel(e.step.name)}return()=>{const i={class:F.value};D.value===!0&&(i.onClick=y,i.onKeyup=d,Object.assign(i,u.value===!0?{tabindex:-1,"aria-disabled":"true"}:{tabindex:t.tabindex||0}));const C=[c("div",{class:"q-focus-helper",tabindex:-1,ref:o}),c("div",{class:"q-stepper__dot row flex-center q-stepper__line relative-position"},[c("span",{class:"row flex-center"},[_.value===!0?e.step.prefix:c(se,{name:f.value})])])];if(e.step.title!==void 0&&e.step.title!==null){const b=[c("div",{class:"q-stepper__title"},e.step.title)];e.step.caption!==void 0&&e.step.caption!==null&&b.push(c("div",{class:"q-stepper__caption"},e.step.caption)),C.push(c("div",{class:"q-stepper__label q-stepper__line relative-position"},b))}return be(c("div",i,C),[[Oe,S.value]])}}});function ou(e){const t=[.06,6,50];return typeof e=="string"&&e.length&&e.split(":").forEach((s,o)=>{const r=parseFloat(s);r&&(t[o]=r)}),t}var lu=je({name:"touch-swipe",beforeMount(e,{value:t,arg:s,modifiers:o}){if(o.mouse!==!0&&W.has.touch!==!0)return;const r=o.mouseCapture===!0?"Capture":"",u={handler:t,sensitivity:ou(s),direction:pe(o),noop:Ke,mouseStart(a){Ce(a,u)&&Re(a)&&(Z(u,"temp",[[document,"mousemove","move",`notPassive${r}`],[document,"mouseup","end","notPassiveCapture"]]),u.start(a,!0))},touchStart(a){if(Ce(a,u)){const n=a.target;Z(u,"temp",[[n,"touchmove","move","notPassiveCapture"],[n,"touchcancel","end","notPassiveCapture"],[n,"touchend","end","notPassiveCapture"]]),u.start(a)}},start(a,n){W.is.firefox===!0&&oe(e,!0);const D=fe(a);u.event={x:D.left,y:D.top,time:Date.now(),mouse:n===!0,dir:!1}},move(a){if(u.event===void 0)return;if(u.event.dir!==!1){ue(a);return}const n=Date.now()-u.event.time;if(n===0)return;const D=fe(a),_=D.left-u.event.x,f=Math.abs(_),B=D.top-u.event.y,F=Math.abs(B);if(u.event.mouse!==!0){if(f<u.sensitivity[1]&&F<u.sensitivity[1]){u.end(a);return}}else if(window.getSelection().toString()!==""){u.end(a);return}else if(f<u.sensitivity[2]&&F<u.sensitivity[2])return;const S=f/n,y=F/n;u.direction.vertical===!0&&f<F&&f<100&&y>u.sensitivity[0]&&(u.event.dir=B<0?"up":"down"),u.direction.horizontal===!0&&f>F&&F<100&&S>u.sensitivity[0]&&(u.event.dir=_<0?"left":"right"),u.direction.up===!0&&f<F&&B<0&&f<100&&y>u.sensitivity[0]&&(u.event.dir="up"),u.direction.down===!0&&f<F&&B>0&&f<100&&y>u.sensitivity[0]&&(u.event.dir="down"),u.direction.left===!0&&f>F&&_<0&&F<100&&S>u.sensitivity[0]&&(u.event.dir="left"),u.direction.right===!0&&f>F&&_>0&&F<100&&S>u.sensitivity[0]&&(u.event.dir="right"),u.event.dir!==!1?(ue(a),u.event.mouse===!0&&(document.body.classList.add("no-pointer-events--children"),document.body.classList.add("non-selectable"),We(),u.styleCleanup=d=>{u.styleCleanup=void 0,document.body.classList.remove("non-selectable");const i=()=>{document.body.classList.remove("no-pointer-events--children")};d===!0?setTimeout(i,50):i()}),u.handler({evt:a,touch:u.event.mouse!==!0,mouse:u.event.mouse,direction:u.event.dir,duration:n,distance:{x:f,y:F}})):u.end(a)},end(a){u.event!==void 0&&(le(u,"temp"),W.is.firefox===!0&&oe(e,!1),u.styleCleanup!==void 0&&u.styleCleanup(!0),a!==void 0&&u.event.dir!==!1&&ue(a),u.event=void 0)}};if(e.__qtouchswipe=u,o.mouse===!0){const a=o.mouseCapture===!0||o.mousecapture===!0?"Capture":"";Z(u,"main",[[e,"mousedown","mouseStart",`passive${a}`]])}W.has.touch===!0&&Z(u,"main",[[e,"touchstart","touchStart",`passive${o.capture===!0?"Capture":""}`],[e,"touchmove","noop","notPassiveCapture"]])},updated(e,t){const s=e.__qtouchswipe;s!==void 0&&(t.oldValue!==t.value&&(typeof t.value!="function"&&s.end(),s.handler=t.value),s.direction=pe(t.modifiers))},beforeUnmount(e){const t=e.__qtouchswipe;t!==void 0&&(le(t,"main"),le(t,"temp"),W.is.firefox===!0&&oe(e,!1),t.styleCleanup!==void 0&&t.styleCleanup(),delete e.__qtouchswipe)}});function Pe(){const e=new Map;return{getCache:function(t,s){return e[t]===void 0?e[t]=s:e[t]},getCacheWithFn:function(t,s){return e[t]===void 0?e[t]=s():e[t]}}}const ru={name:{required:!0},disable:Boolean},ge={setup(e,{slots:t}){return()=>c("div",{class:"q-panel scroll",role:"tabpanel"},M(t.default))}},iu={modelValue:{required:!0},animated:Boolean,infinite:Boolean,swipeable:Boolean,vertical:Boolean,transitionPrev:String,transitionNext:String,transitionDuration:{type:[String,Number],default:300},keepAlive:Boolean,keepAliveInclude:[String,Array,RegExp],keepAliveExclude:[String,Array,RegExp],keepAliveMax:Number},su=["update:modelValue","beforeTransition","transition"];function cu(){const{props:e,emit:t,proxy:s}=U(),{getCacheWithFn:o}=Pe();let r,u;const a=N(null),n=N(null);function D(v){const A=e.vertical===!0?"up":"left";V((s.$q.lang.rtl===!0?-1:1)*(v.direction===A?1:-1))}const _=g(()=>[[lu,D,void 0,{horizontal:e.vertical!==!0,vertical:e.vertical,mouse:!0}]]),f=g(()=>e.transitionPrev||`slide-${e.vertical===!0?"down":"right"}`),B=g(()=>e.transitionNext||`slide-${e.vertical===!0?"up":"left"}`),F=g(()=>`--q-transition-duration: ${e.transitionDuration}ms`),S=g(()=>typeof e.modelValue=="string"||typeof e.modelValue=="number"?e.modelValue:String(e.modelValue)),y=g(()=>({include:e.keepAliveInclude,exclude:e.keepAliveExclude,max:e.keepAliveMax})),d=g(()=>e.keepAliveInclude!==void 0||e.keepAliveExclude!==void 0);ee(()=>e.modelValue,(v,A)=>{const P=H(v)===!0?K(v):-1;u!==!0&&Y(P===-1?0:P<K(A)?-1:1),a.value!==P&&(a.value=P,t("beforeTransition",v,A),Ue(()=>{t("transition",v,A)}))});function i(){V(1)}function C(){V(-1)}function b(v){t("update:modelValue",v)}function H(v){return v!=null&&v!==""}function K(v){return r.findIndex(A=>A.props.name===v&&A.props.disable!==""&&A.props.disable!==!0)}function z(){return r.filter(v=>v.props.disable!==""&&v.props.disable!==!0)}function Y(v){const A=v!==0&&e.animated===!0&&a.value!==-1?"q-transition--"+(v===-1?f.value:B.value):null;n.value!==A&&(n.value=A)}function V(v,A=a.value){let P=A+v;for(;P>-1&&P<r.length;){const O=r[P];if(O!==void 0&&O.props.disable!==""&&O.props.disable!==!0){Y(v),u=!0,t("update:modelValue",O.props.name),setTimeout(()=>{u=!1});return}P+=v}e.infinite===!0&&r.length!==0&&A!==-1&&A!==r.length&&V(v,v===-1?r.length:-1)}function R(){const v=K(e.modelValue);return a.value!==v&&(a.value=v),!0}function G(){const v=H(e.modelValue)===!0&&R()&&r[a.value];return e.keepAlive===!0?[c(Se,y.value,[c(d.value===!0?o(S.value,()=>({...ge,name:S.value})):ge,{key:S.value,style:F.value},()=>v)])]:[c("div",{class:"q-panel scroll",style:F.value,key:S.value,role:"tabpanel"},[v])]}function te(){if(r.length!==0)return e.animated===!0?[c(De,{name:n.value},G)]:G()}function X(v){return r=ze(M(v.default,[])).filter(A=>A.props!==null&&A.props.slot===void 0&&H(A.props.name)===!0),r.length}function ne(){return r}return Object.assign(s,{next:i,previous:C,goTo:b}),{panelIndex:a,panelDirectives:_,updatePanelsList:X,updatePanelIndex:R,getPanelContent:te,getEnabledPanels:z,getPanels:ne,isValidPanelName:H,keepAliveProps:y,needsUniqueKeepAliveWrapper:d,goToPanelByOffset:V,goToPanel:b,nextPanel:i,previousPanel:C}}function Te(e){return c("div",{class:"q-stepper__step-content"},[c("div",{class:"q-stepper__step-inner"},M(e.default))])}const Be={setup(e,{slots:t}){return()=>Te(t)}};var $=j({name:"QStep",props:{...ru,icon:String,color:String,title:{type:String,required:!0},caption:String,prefix:[String,Number],doneIcon:String,doneColor:String,activeIcon:String,activeColor:String,errorIcon:String,errorColor:String,headerNav:{type:Boolean,default:!0},done:Boolean,error:Boolean,onScroll:[Function,Array]},setup(e,{slots:t,emit:s}){const{proxy:{$q:o}}=U(),r=Ye(xe,re);if(r===re)return console.error("QStep needs to be a child of QStepper"),re;const{getCacheWithFn:u}=Pe(),a=N(null),n=g(()=>r.value.modelValue===e.name),D=g(()=>o.platform.is.ios!==!0&&o.platform.is.chrome===!0||n.value!==!0||r.value.vertical!==!0?{}:{onScroll(B){const{target:F}=B;F.scrollTop>0&&(F.scrollTop=0),e.onScroll!==void 0&&s("scroll",B)}}),_=g(()=>typeof e.name=="string"||typeof e.name=="number"?e.name:String(e.name));function f(){const B=r.value.vertical;return B===!0&&r.value.keepAlive===!0?c(Se,r.value.keepAliveProps.value,n.value===!0?[c(r.value.needsUniqueKeepAliveWrapper.value===!0?u(_.value,()=>({...Be,name:_.value})):Be,{key:_.value},t.default)]:void 0):B!==!0||n.value===!0?Te(t):void 0}return()=>c("div",{ref:a,class:"q-stepper__step",role:"tabpanel",...D.value},r.value.vertical===!0?[c(ke,{stepper:r.value,step:e,goToPanel:r.value.goToPanel}),r.value.animated===!0?c(we,f):f()]:[f()])}});const du=/(-\w)/g;function vu(e){const t={};for(const s in e){const o=s.replace(du,r=>r[1].toUpperCase());t[o]=e[s]}return t}var ie=j({name:"QStepper",props:{..._e,...iu,flat:Boolean,bordered:Boolean,alternativeLabels:Boolean,headerNav:Boolean,contracted:Boolean,headerClass:String,inactiveColor:String,inactiveIcon:String,doneIcon:String,doneColor:String,activeIcon:String,activeColor:String,errorIcon:String,errorColor:String},emits:su,setup(e,{slots:t}){const s=U(),o=Ae(e,s.proxy.$q),{updatePanelsList:r,isValidPanelName:u,updatePanelIndex:a,getPanelContent:n,getPanels:D,panelDirectives:_,goToPanel:f,keepAliveProps:B,needsUniqueKeepAliveWrapper:F}=cu();Ge(xe,g(()=>({goToPanel:f,keepAliveProps:B,needsUniqueKeepAliveWrapper:F,...e})));const S=g(()=>`q-stepper q-stepper--${e.vertical===!0?"vertical":"horizontal"}`+(e.flat===!0?" q-stepper--flat":"")+(e.bordered===!0?" q-stepper--bordered":"")+(o.value===!0?" q-stepper--dark q-dark":"")),y=g(()=>`q-stepper__header row items-stretch justify-between q-stepper__header--${e.alternativeLabels===!0?"alternative":"standard"}-labels`+(e.flat===!1||e.bordered===!0?" q-stepper__header--border":"")+(e.contracted===!0?" q-stepper__header--contracted":"")+(e.headerClass!==void 0?` ${e.headerClass}`:""));function d(){const i=M(t.message,[]);if(e.vertical===!0){u(e.modelValue)&&a();const C=c("div",{class:"q-stepper__content"},M(t.default));return i===void 0?[C]:i.concat(C)}return[c("div",{class:y.value},D().map(C=>{const b=vu(C.props);return c(ke,{key:b.name,stepper:e,step:b,goToPanel:f})})),i,Je("div",{class:"q-stepper__content q-panel-parent"},n(),"cont",e.swipeable,()=>_.value)]}return()=>(r(t),c("div",{class:S.value},Xe(t.navigation,d())))}});const E=e=>(uu("data-v-f647adca"),e=e(),tu(),e),pu=E(()=>m("h1",null,"ServerStarter\u3078\u3088\u3046\u3053\u305D\uFF01",-1)),Cu=E(()=>m("p",null,"ServerStarter\u306FMinecraft\u306E\u30DE\u30EB\u30C1\u30B5\u30FC\u30D0\u30FC\u3092\u30DC\u30BF\u30F3\u30AF\u30EA\u30C3\u30AF\u306B\u3088\u3063\u3066\u7C21\u5358\u306B\u7ACB\u3066\u3089\u308C\u308B\u3088\u3046\u306B\u3059\u308B\u30BD\u30D5\u30C8\u30A6\u30A7\u30A2\u3067\u3059",-1)),fu=E(()=>m("p",null,"\u30EF\u30F3\u30AF\u30EA\u30C3\u30AF\u3067\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\uFF0C\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u306E\u4E16\u754C\u306B\u30C0\u30A4\u30D6\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),Fu=E(()=>m("h1",null,"\u5C0E\u5165\u65B9\u6CD5",-1)),mu=E(()=>m("p",null,"ServerStarter\u306E\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u3092\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9",-1)),gu=E(()=>m("p",null,"\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u3092\u8D77\u52D5\u3057\u3066ServerStarter\u3092PC\u306B\u30A4\u30F3\u30B9\u30C8\u30FC\u30EB",-1)),Bu={class:"q-pb-md"},hu=E(()=>m("p",null,[k(" ServerStarter\u306F\u500B\u4EBA\u958B\u767A\u306E\u305F\u3081\uFF0C\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6570\u304C\u5341\u5206\u306A\u6570\u306B\u306A\u308B\u307E\u3067\u76F8\u5F53\u306E\u6642\u9593\u304C\u304B\u304B\u308A\u307E\u3059"),m("br"),k(" \u4E00\u90E8\u306E\u30D6\u30E9\u30A6\u30B6\u3067\u306F\u5341\u5206\u306A\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6570\u306E\u306A\u3044\u30BD\u30D5\u30C8\u306B\u5BFE\u3057\u3066\u8B66\u544A\u3092\u51FA\u3059\u305F\u3081\uFF0C\u30BD\u30D5\u30C8\u3092\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u3067\u304D\u306A\u3044\u3053\u3068\u304C\u3042\u308A\u307E\u3059 ")],-1)),Du=E(()=>m("p",null,"Edge\u3067\u306F\u753B\u50CF\u306E\u3088\u3046\u306B\u53F3\u5074\u306E\u300C\u30FB\u30FB\u30FB\u300D\u304B\u3089\u4FDD\u5B58\u3092\u62BC\u3057\uFF0C\u6B21\u306E\u753B\u9762\u3067\u300C\u4FDD\u6301\u3059\u308B\u300D\u3092\u30AF\u30EA\u30C3\u30AF\u3059\u308B\u3053\u3068\u3067\uFF0C\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u306E\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u304C\u59CB\u307E\u308A\u307E\u3059",-1)),Eu=E(()=>m("p",null,[k(" \u306A\u304A\uFF0CServerStarter\u306E\u30BD\u30FC\u30B9\u30B3\u30FC\u30C9\u306F "),m("a",{href:"https://github.com/CivilTT/ServerStarter",target:"_blank",class:"a"},"GitHub"),k(" \u306B\u3066\u516C\u958B\u3057\u3066\u304A\u308A\u307E\u3059\u306E\u3067\uFF0C\u305D\u3061\u3089\u3082\u3054\u78BA\u8A8D\u3044\u305F\u3060\u3051\u307E\u3059\u3068\u5E78\u3044\u3067\u3059 ")],-1)),_u=E(()=>m("p",null,"\u30C7\u30B9\u30AF\u30C8\u30C3\u30D7\u306B\u30A2\u30A4\u30B3\u30F3\u304C\u4F5C\u6210\u3055\u308C\u308B\u305F\u3081\uFF0C\u3053\u308C\u3092\u30C0\u30D6\u30EB\u30AF\u30EA\u30C3\u30AF\u3057\u3066\u8D77\u52D5",-1)),Au=E(()=>m("p",null,"\u8A00\u8A9E\u8A2D\u5B9A\u3092\u78BA\u8A8D\u3057\u3001\u5229\u7528\u898F\u7D04\u306B\u540C\u610F\u3059\u308C\u3070\u300C\u30B9\u30BF\u30FC\u30C8\u300D\uFF01",-1)),bu=E(()=>m("p",null,"Minecraft\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u30A2\u30AB\u30A6\u30F3\u30C8\u3092\u6301\u3063\u3066\u3044\u308B\u5834\u5408\u306F\u3001\u30B2\u30FC\u30E0\u5185\u3067\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u540D\u3092\u767B\u9332\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),Su=E(()=>m("p",null,[k(" \u81EA\u8EAB\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u540D\u3092\u5165\u529B\u3059\u308B\u3068\u3001\u4EE5\u4E0B\u306E\u753B\u50CF\u306E\u3088\u3046\u306B\u5019\u88DC\u304C\u8868\u793A\u3055\u308C\u307E\u3059"),m("br"),k(" \u300C\u3053\u306E\u30D7\u30EC\u30A4\u30E4\u30FC\u3092\u767B\u9332\u300D\u30DC\u30BF\u30F3\u3092\u62BC\u3057\u3001\u300C\u30AA\u30FC\u30CA\u30FC\u3092\u767B\u9332\u300D\u3067\u304D\u308B\u3088\u3046\u306B\u306A\u308A\u307E\u3059")],-1)),xu=E(()=>m("h1",null,"\u30B5\u30FC\u30D0\u30FC\u306E\u8D77\u52D5",-1)),yu=E(()=>m("p",null,"\u30EF\u30FC\u30EB\u30C9\u540D\u3092\u597D\u304D\u306A\u540D\u79F0\u306B\u5909\u66F4\u3057\u3001\u30B5\u30FC\u30D0\u30FC\u306E\u30D0\u30FC\u30B8\u30E7\u30F3\u6307\u5B9A\u3001\u30B7\u30F3\u30B0\u30EB\u30EF\u30FC\u30EB\u30C9\u306E\u5C0E\u5165\u8A2D\u5B9A\u306A\u3069\u304C\u3067\u304D\u307E\u3059",-1)),Iu=E(()=>m("p",null,"\u30D7\u30ED\u30D1\u30C6\u30A3\u30BF\u30B0\u3084\u30D7\u30EC\u30A4\u30E4\u30FC\u30BF\u30B0\u3067\u3082\u30B5\u30FC\u30D0\u30FC\u306E\u8A73\u7D30\u306A\u8A2D\u5B9A\u3092\u884C\u3046\u3053\u3068\u304C\u3067\u304D\u307E\u3059\uFF01",-1)),qu=E(()=>m("p",null,"\u6E96\u5099\u304C\u6574\u3063\u305F\u3089\u3001\u300C\u30B5\u30FC\u30D0\u30FC\u30BF\u30B0\u300D\u3092\u958B\u3044\u3066\u3001\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\u307E\u3057\u3087\u3046\uFF01",-1)),wu=E(()=>m("p",{class:"q-pt-md"},[k(" \u30B5\u30FC\u30D0\u30FC\u304C\u8D77\u52D5\u3059\u308B\u3068\u3053\u306E\u3088\u3046\u306A\u753B\u9762\u304C\u8868\u793A\u3055\u308C\u307E\u3059"),m("br"),k(" \u521D\u56DE\u8D77\u52D5\u6642\u306F\u8D77\u52D5\u524D\u306BEula\u3078\u306E\u540C\u610F\u3092\u6C42\u3081\u3089\u308C\u307E\u3059"),m("br"),k(" \u7D42\u308F\u3063\u305F\u3068\u304D\u306B\u306F\u5DE6\u4E0B\u306E\u300C\u505C\u6B62\u300D\u30DC\u30BF\u30F3\u3092\u62BC\u3057\u3066\u30B5\u30FC\u30D0\u30FC\u3092\u9589\u3058\u307E\u3057\u3087\u3046\uFF01 ")],-1)),ku=E(()=>m("h1",null,"\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0",-1)),Pu=E(()=>m("p",null," Minecraft Launcher\u3092\u8D77\u52D5\u3057\uFF0C\u30B5\u30FC\u30D0\u30FC\u3092\u8D77\u52D5\u3057\u305F\u30D0\u30FC\u30B8\u30E7\u30F3\u3067\u30B2\u30FC\u30E0\u3092\u958B\u59CB\u3059\u308B ",-1)),Tu=E(()=>m("p",null," \u5FC5\u8981\u306B\u5FDC\u3058\u3066\u300C\u8D77\u52D5\u69CB\u6210\u300D\u304B\u3089\u8D77\u52D5\u3057\u305F\u3044\u30D0\u30FC\u30B8\u30E7\u30F3\u3092\u8FFD\u52A0\u3059\u308B ",-1)),Lu=E(()=>m("p",null," \u8D77\u52D5\u3059\u308B\u3068\u30B7\u30F3\u30B0\u30EB\u30D7\u30EC\u30A4\u306E\u4E0B\u306B\u3042\u308B\u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u3092\u9078\u629E\u3057\uFF0C\u53F3\u4E0B\u306E\u300C\u30B5\u30FC\u30D0\u30FC\u3092\u8FFD\u52A0\u300D\u3092\u62BC\u3059 ",-1)),$u=E(()=>m("p",null," \u5206\u304B\u308A\u3084\u3059\u3044\u30B5\u30FC\u30D0\u30FC\u540D\u3092\u8A2D\u5B9A\u3057\uFF0C\u30B5\u30FC\u30D0\u30FC\u30A2\u30C9\u30EC\u30B9\u306B\u306F\u300Clocalhost\u300D\u3068\u8A18\u5165 ",-1)),Nu=E(()=>m("p",null," \u300C\u5B8C\u4E86\u300D\u3092\u62BC\u305B\u3070\u30EF\u30FC\u30EB\u30C9\u306B\u5165\u308B\u3053\u3068\u304C\u3067\u304D\u308B ",-1)),Vu=E(()=>m("p",null," \u30B5\u30FC\u30D0\u30FC\u306E\u8D77\u52D5\u8005\u3067\u306A\u3044\u4EBA\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0\u3059\u308B\u969B\u306B\u306F\u3001ServerStarter\u306E\u753B\u9762\u53F3\u4E0A\u306B\u6620\u3063\u3066\u3044\u308BIP\u30A2\u30C9\u30EC\u30B9\u3092\u3001Minecraft\u306E\u30B5\u30FC\u30D0\u30FC\u30A2\u30C9\u30EC\u30B9\u6B04\u306B\u5165\u529B\u3057\u3066\u304F\u3060\u3055\u3044 ",-1)),Qu={class:"row q-gutter-md"},Mu=ye({__name:"HowToStart",setup(e){var t=N(1),s=N(1),o=N(1);return(r,u)=>{const a=Ze("router-link");return Ie(),qe(Fe,{flat:"",style:{"max-width":"100%"}},{default:h(()=>[l(me,null,{default:h(()=>[pu,Cu,fu,Fu,l(ie,{modelValue:w(t),"onUpdate:modelValue":u[6]||(u[6]=n=>x(t)?t.value=n:t=n),flat:"",vertical:"",animated:""},{default:h(()=>[l($,{name:1,title:"1. \u5C0E\u5165",done:w(t)>1},{default:h(()=>[mu,gu,m("div",Bu,[l(au,{label:"\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9\u6642\u306B\u8B66\u544A\u304C\u51FA\u305F\u5834\u5408","header-class":"bg-orange-4"},{default:h(()=>[l(Fe,{flat:"",class:"bg-orange-2"},{default:h(()=>[l(me,null,{default:h(()=>[hu,Du,Eu,l(I,{src:"src/assets/Introduction/Edge_Save1.png",width:"min(400px,100%)"}),l(I,{src:"src/assets/Introduction/Edge_Save2.png",width:"min(400px,100%)"})]),_:1})]),_:1})]),_:1})]),l(q,{size:"20px",color:"primary",label:"\u30A4\u30F3\u30B9\u30C8\u30FC\u30E9\u30FC\u306E\u30C0\u30A6\u30F3\u30ED\u30FC\u30C9",href:"https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"}),l(L,null,{default:h(()=>[l(q,{onClick:u[0]||(u[0]=n=>x(t)?t.value=2:t=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),l($,{name:2,title:"2. \u8D77\u52D5",done:w(t)>2},{default:h(()=>[_u,l(I,{src:"src/assets/Introduction/Icon.png",width:"min(200px,100%)"}),l(L,null,{default:h(()=>[l(q,{onClick:u[1]||(u[1]=n=>x(t)?t.value=3:t=3),color:"secondary",label:"next"}),l(q,{flat:"",onClick:u[2]||(u[2]=n=>x(t)?t.value=1:t=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),l($,{name:3,title:"3-1. \u521D\u671F\u8A2D\u5B9A\uFF08\u5229\u7528\u898F\u7D04\uFF09",done:w(t)>3},{default:h(()=>[Au,l(I,{src:"src/assets/Introduction/WelcomeWindow.png",width:"min(500px,100%)"}),l(L,null,{default:h(()=>[l(q,{onClick:u[3]||(u[3]=n=>x(t)?t.value=4:t=4),color:"secondary",label:"next"}),l(q,{flat:"",onClick:u[4]||(u[4]=n=>x(t)?t.value=2:t=2),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),l($,{name:4,title:"3-2. \u521D\u671F\u8A2D\u5B9A\uFF08\u30D7\u30EC\u30A4\u30E4\u30FC\u767B\u9332\uFF09",done:w(t)>3},{default:h(()=>[bu,l(I,{src:"src/assets/Introduction/OwnerPlayerSetting1.png",width:"min(500px,100%)"}),Su,l(I,{src:"src/assets/Introduction/OwnerPlayerSetting2.png",width:"min(500px,100%)"}),l(L,null,{default:h(()=>[l(q,{flat:"",onClick:u[5]||(u[5]=n=>x(t)?t.value=3:t=3),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"]),xu,l(ie,{modelValue:w(s),"onUpdate:modelValue":u[9]||(u[9]=n=>x(s)?s.value=n:s=n),flat:"",vertical:"",animated:""},{default:h(()=>[l($,{name:1,title:"1. \u8A2D\u5B9A",done:w(s)>1},{default:h(()=>[yu,Iu,l(I,{src:"src/assets/Introduction/MainWindow.png",width:"min(900px,100%)"}),l(L,null,{default:h(()=>[l(q,{onClick:u[7]||(u[7]=n=>x(s)?s.value=2:s=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),l($,{name:2,title:"2. \u8D77\u52D5",done:w(s)>2},{default:h(()=>[qu,l(I,{src:"src/assets/Introduction/Server1.png",width:"min(700px,100%)"}),wu,l(I,{src:"src/assets/Introduction/Server2.png",width:"min(700px,100%)"}),l(L,null,{default:h(()=>[l(q,{flat:"",onClick:u[8]||(u[8]=n=>x(s)?s.value=1:s=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"]),ku,l(ie,{modelValue:w(o),"onUpdate:modelValue":u[14]||(u[14]=n=>x(o)?o.value=n:o=n),flat:"",vertical:"",animated:""},{default:h(()=>[l($,{name:1,title:"1. Minecraft\u306E\u8D77\u52D5",done:w(o)>1},{default:h(()=>[Pu,Tu,l(I,{src:"src/assets/Introduction/Launcher.png",width:"min(500px,100%)"}),l(L,null,{default:h(()=>[l(q,{onClick:u[10]||(u[10]=n=>x(o)?o.value=2:o=2),color:"secondary",label:"next"})]),_:1})]),_:1},8,["done"]),l($,{name:2,title:"2. \u30DE\u30EB\u30C1\u30D7\u30EC\u30A4\u306E\u8A2D\u5B9A",done:w(o)>2},{default:h(()=>[Lu,$u,Nu,l(I,{src:"src/assets/Introduction/client.png",width:"min(500px,100%)"}),l(L,null,{default:h(()=>[l(q,{onClick:u[11]||(u[11]=n=>x(o)?o.value=3:o=3),color:"secondary",label:"next"}),l(q,{flat:"",onClick:u[12]||(u[12]=n=>x(o)?o.value=1:o=1),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"]),l($,{name:3,title:"3. \u307B\u304B\u306E\u4EBA\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0\u3059\u308B",done:w(o)>3},{default:h(()=>[m("p",null,[k(" \u4ED6\u306E\u4EBA\u304C\u30B5\u30FC\u30D0\u30FC\u306B\u53C2\u52A0\u3059\u308B\u305F\u3081\u306B\u306F\u30B5\u30FC\u30D0\u30FC\u306E\u8D77\u52D5\u8005\u306B"),l(a,{class:"a",to:"/PortMapping"},{default:h(()=>[k("\u30DD\u30FC\u30C8\u958B\u653E")]),_:1}),k("\u306E\u8A2D\u5B9A\u304C\u5FC5\u8981\u3067\u3059 ")]),Vu,m("div",Qu,[l(I,{src:"src/assets/Introduction/ip.png",width:"min(500px,100%)"}),l(I,{src:"src/assets/Introduction/client2.png",width:"min(500px,100%)"})]),l(L,null,{default:h(()=>[l(q,{flat:"",onClick:u[13]||(u[13]=n=>x(o)?o.value=2:o=2),color:"secondary",label:"Back",class:"q-ml-sm"})]),_:1})]),_:1},8,["done"])]),_:1},8,["modelValue"])]),_:1})]),_:1})}}});var Hu=eu(Mu,[["__scopeId","data-v-f647adca"]]);const Ku=ye({__name:"IntroductionView",setup(e){return(t,s)=>(Ie(),qe(Hu))}});export{Ku as default};
