import{c as d,j as I,r as s,w as $,A as j,h as o,B as Q,m as L,Q as M,n as U,C as D,q as E,s as F,t as H,D as O}from"./index.68a2dfa6.js";const P={ratio:[String,Number]};function A(e,l){return d(()=>{const n=Number(e.ratio||(l!==void 0?l.value:void 0));return isNaN(n)!==!0&&n>0?{paddingBottom:`${100/n}%`}:null})}const W=16/9;var x=I({name:"QImg",props:{...P,src:String,srcset:String,sizes:String,alt:String,crossorigin:String,decoding:String,referrerpolicy:String,draggable:Boolean,loading:{type:String,default:"lazy"},fetchpriority:{type:String,default:"auto"},width:String,height:String,initialRatio:{type:[Number,String],default:W},placeholderSrc:String,fit:{type:String,default:"cover"},position:{type:String,default:"50% 50%"},imgClass:String,imgStyle:Object,noSpinner:Boolean,noNativeMenu:Boolean,noTransition:Boolean,spinnerColor:String,spinnerSize:String},emits:["load","error"],setup(e,{slots:l,emit:n}){const f=s(e.initialRatio),u=A(e,f);let i=null,m=!1;const a=[s(null),s(S())],r=s(0),c=s(!1),g=s(!1),C=d(()=>`q-img q-img--${e.noNativeMenu===!0?"no-":""}menu`),q=d(()=>({width:e.width,height:e.height})),B=d(()=>`q-img__image ${e.imgClass!==void 0?e.imgClass+" ":""}q-img__image--with${e.noTransition===!0?"out":""}-transition`),T=d(()=>({...e.imgStyle,objectFit:e.fit,objectPosition:e.position}));$(()=>y(),_);function y(){return e.src||e.srcset||e.sizes?{src:e.src,srcset:e.srcset,sizes:e.sizes}:null}function S(){return e.placeholderSrc!==void 0?{src:e.placeholderSrc}:null}function _(t){i!==null&&(clearTimeout(i),i=null),g.value=!1,t===null?(c.value=!1,a[r.value^1].value=S()):c.value=!0,a[r.value].value=t}function k({target:t}){m!==!0&&(i!==null&&(clearTimeout(i),i=null),f.value=t.naturalHeight===0?.5:t.naturalWidth/t.naturalHeight,w(t,1))}function w(t,v){m===!0||v===1e3||(t.complete===!0?z(t):i=setTimeout(()=>{i=null,w(t,v+1)},50))}function z(t){m!==!0&&(r.value=r.value^1,a[r.value].value=null,c.value=!1,g.value=!1,n("load",t.currentSrc||t.src))}function R(t){i!==null&&(clearTimeout(i),i=null),c.value=!1,g.value=!0,a[r.value].value=null,a[r.value^1].value=S(),n("error",t)}function b(t){const v=a[t].value,h={key:"img_"+t,class:B.value,style:T.value,crossorigin:e.crossorigin,decoding:e.decoding,referrerpolicy:e.referrerpolicy,height:e.height,width:e.width,loading:e.loading,fetchpriority:e.fetchpriority,"aria-hidden":"true",draggable:e.draggable,...v};return r.value===t?(h.class+=" q-img__image--waiting",Object.assign(h,{onLoad:k,onError:R})):h.class+=" q-img__image--loaded",o("div",{class:"q-img__container absolute-full",key:"img"+t},o("img",h))}function N(){return c.value!==!0?o("div",{key:"content",class:"q-img__content absolute-full q-anchor--skip"},L(l[g.value===!0?"error":"default"])):o("div",{key:"loading",class:"q-img__loading absolute-full flex flex-center"},l.loading!==void 0?l.loading():e.noSpinner===!0?void 0:[o(M,{color:e.spinnerColor,size:e.spinnerSize})])}return _(y()),j(()=>{m=!0,i!==null&&(clearTimeout(i),i=null)}),()=>{const t=[];return u.value!==null&&t.push(o("div",{key:"filler",style:u.value})),g.value!==!0&&(a[0].value!==null&&t.push(b(0)),a[1].value!==null&&t.push(b(1))),t.push(o(Q,{name:"q-transition--fade"},N)),o("div",{class:C.value,style:q.value,role:"img","aria-label":e.alt},t)}}});const J=U({__name:"SsImg",props:{path:{},width:{}},setup(e){const l=e,n=s("");async function f(){const u=window.location.origin+"/ServerStarter/";n.value=new URL(`${l.path}`,u).href}return D(f),(u,i)=>(E(),F(x,{src:n.value,width:u.width},{default:H(()=>[O(u.$slots,"default")]),_:3},8,["src","width"]))}});export{x as Q,J as _};