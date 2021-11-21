import React, {Component} from 'react';
import Loading from "../helpers/Loading";
import {connect} from "react-redux";
import {saveBrowsePlaylists} from "../../store/actions/playlistActions";
import PlaylistCardContainer from "./PlaylistCardContainer"
import {logout} from "../../store/actions/authActions";
import * as api from "../../infrastructure/api/api-gateway";

class BrowsePlaylists extends Component {

    state = {
        loading: true
    };

    componentDidMount() {
        if (!this.props.isLoggedIn || this.props.browsePlaylists) {

            this.setState({
                loading: false
            });

            return;
        }

        api.getBrowsePlaylists()
            .then(res => {
                if (res.status === 200 && !res.data.hasError) {
                    this.props.saveBrowsePlaylists(res.data.result);
                }

                this.setState({
                    loading: false
                });
            })
            .catch(error => {

                if (error.response.status === 401) {
                    this.props.logout();
                }

                this.setState({
                    loading: false
                });
            });
    }

    render() {
        return this.state.loading
            ? <Loading text="Getting your playlists"/>
            : this.props.browsePlaylists
                ? <PlaylistCardContainer playlists={this.props.browsePlaylists}/>
                : <Loading text="User has no playlists. We're waiting for you to create a playlist!"/>
    }
}

const mapStateToProps = state => {
    return {
        isLoggedIn: state.auth.isLoggedIn,
        browsePlaylists: state.playlist.browsePlaylists
    };
};

const mapDispatchToProps = dispatch => {
    return {
        saveBrowsePlaylists: playlists => dispatch(saveBrowsePlaylists(playlists)),
        logout: () => dispatch(logout())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(BrowsePlaylists);
