import React from 'react';
import {Roller} from 'react-spinners-css';
import {errorLoadingColor, loadingColor, white} from "../../infrastructure/constants/color-constants";
import {redirectMessage} from "../../infrastructure/constants/error-constants";

const Loading = props => {
    return (
        <div style={{height: 750}}>
            <div style={{marginTop: '15%', textAlign: 'center'}}>
                {
                    props.isErr
                        ? <h2 style={{color: errorLoadingColor}}>{redirectMessage}</h2>
                        : null
                }
                <Roller color={white} size={350}/>
                <h2 style={{color: props.isErr ? errorLoadingColor : loadingColor}}>{props.text}</h2>
            </div>
        </div>
    )
};

export default Loading;
