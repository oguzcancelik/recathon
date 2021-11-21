import React, {Component} from 'react';
import Loading from "./Loading";
import {Redirect} from "react-router-dom";

class Error extends Component {

    state = {
        loading: true
    };

    componentDidMount() {
        setTimeout(() =>
            this.setState({
                loading: false
            }), 3000);
    }

    render() {
        return this.state.loading
            ? <Loading text={this.props.message} isErr={true}/>
            : <Redirect to='/'/>;
    }
}

export default Error;
